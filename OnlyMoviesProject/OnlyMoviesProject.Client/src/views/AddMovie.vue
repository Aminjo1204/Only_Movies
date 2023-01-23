<script setup>
import axios from 'axios'; // npm install axios
import Spinner from '../components/Spinner.vue';
</script>

<template>
    <div class="addMovieView">
        <h3>Load movies from OMDb API</h3>
        <Spinner v-if="loading"></Spinner>
        <p class="mb-3">
            Search <a href="https://www.omdbapi.com" target="_blank">omdbapi</a> to add movies to the database.
            <span class="mb-3" v-if="!authenticated">Please <router-link to="/login">log in</router-link> to add movies to the database.</span>
        </p>

        <div class="formRow">
            <label>Title:</label>
            <input class="form-control" type="text" v-on:keyup="searchOnEnter($event)" v-model="title" />
            <button class="btn btn-outline-primary" v-on:click="searchForMovies()">Search</button>
        </div>
        <template v-if="movies.length">
            <h4>{{ movies.length }} Movies found.</h4>
            <div class="movielist" v-if="movies.length">
                <div class="movieitem" v-for="m in movies" v-bind:key="m.ImdbID">
                    <div class="movieposter">
                        <img v-bind:src="m.Poster" />
                    </div>
                    <div class="movieinfo">
                        <div class="movietitle">{{ m.Title }} {{ m.Year }}</div>
                        <div v-if="authenticated" class="addbutton"><button class="btn btn-outline-primary btn-sm" v-on:click="addMovie(m.imdbID)">Add</button></div>
                    </div>
                </div>
            </div>
        </template>
    </div>
</template>

<style scoped>
.formRow {
    display: flex;
    align-items: center;
    margin-bottom: 1em;
}
.formRow label {
    display: block;
    flex: 0 0 4em;
}
.movielist {
    display: grid;
    grid-template-columns: 1fr 1fr 1fr;
    gap: 1em;
}
.movielist :hover {
    background-color: hsl(180, 53%, 90%);
}
.movieitem {
    display: flex;
    gap: 1em;
}

.movieposter img {
    width: 100px;
}

.movieinfo {
    display: flex;
    flex-direction: column;
}

.movietitle {
    flex-grow: 1;
}
</style>

<script>
export default {
    components: {
        Spinner,
    },
    data() {
        return {
            title: 'Avatar',
            movies: [],
            loading: false,
        };
    },
    mounted() {
        const state = JSON.parse(sessionStorage.getItem('addMovieState')) || {};
        this.title = state.title || 'Avatar';
        this.movies = state.movies || [];
    },
    methods: {
        async searchForMovies() {
            try {
                if (!this.title) return;
                this.loading = true;
                // Create a new instance of axios because our default instance holds an auth
                // token for our api.
                const axiosOmdb = axios.create();
                axiosOmdb.defaults.headers.common = {};
                // Query https://www.omdbapi.com for movies.
                const response = await axiosOmdb.get('https://www.omdbapi.com', { params: { apikey: 'cd2aa4ca', s: this.title } });
                const movies = response.data.Search || [];
                this.movies = movies.filter((m) => m.Type == 'movie');
                sessionStorage.setItem('addMovieState', JSON.stringify({ title: this.title, movies: this.movies }));
            } catch (e) {
                alert('Error fetching omdbapi.');
                alert(e);
            } finally {
                this.loading = false;
            }
        },
        searchOnEnter(e) {
            if (e.key !== 'Enter') {
                return;
            }
            this.searchForMovies();
        },
        async addMovie(movieId) {
            try {
                this.loading = true;
                const axiosOmdb = axios.create();
                axiosOmdb.defaults.headers.common = {};
                const response = await axiosOmdb.get('https://www.omdbapi.com', { params: { apikey: 'cd2aa4ca', i: movieId } });
                const movie = response.data;
                if (!movie.Title) {
                    alert('Cannot fetch movie details from omdbapi.');
                    return;
                }
                const runtime = /(?<mins>\d+) min/.exec(movie.Runtime);
                movie.imdbId = movieId;
                movie.genres = movie.genre;
                try {
                    await axios.post('movie', {
                        title: movie.Title,
                        imdbId: movieId,
                        plot: movie.Plot,
                        lengthSec: runtime == null ? null : runtime.groups.mins * 60,
                        releaseDate: new Date(movie.Released).toISOString().substr(0, 10),
                        genres: movie.Genre.split(/,\s+/),
                        actors: movie.Actors.split(/,\s+/),
                        rated: movie.Rated == 'N/A' ? null : movie.Rated,
                        imageUrl: movie.Poster,
                    });
                } catch (e) {
                    if (e.response.status == 422) {
                        alert(`The movie ${movie.Title} is already in the database.`);
                        return;
                    }
                    alert(JSON.stringify(e.response.data));
                }
            } catch (e) {
                alert('Cannot fetch movie details from omdbapi.');
                return;
            } finally {
                this.loading = false;
            }
        },
    },
    computed: {
        authenticated() {
            return this.$store.state.userdata.username ? true : false;
        },
        userdata() {
            return this.$store.state.userdata;
        },
    },
};
</script>