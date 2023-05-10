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
                        <div class="addbutton"><button class="btn btn-outline-primary btn-sm" v-on:click="addMovie(m.imdbID)">Add</button></div>
                        <label class="container">

                    <input type="checkbox">
                    <svg height="24px" id="Layer_1" version="1.2" viewBox="0 0 24 24" width="24px" xml:space="preserve" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink"><g><g><path d="M9.362,9.158c0,0-3.16,0.35-5.268,0.584c-0.19,0.023-0.358,0.15-0.421,0.343s0,0.394,0.14,0.521    c1.566,1.429,3.919,3.569,3.919,3.569c-0.002,0-0.646,3.113-1.074,5.19c-0.036,0.188,0.032,0.387,0.196,0.506    c0.163,0.119,0.373,0.121,0.538,0.028c1.844-1.048,4.606-2.624,4.606-2.624s2.763,1.576,4.604,2.625    c0.168,0.092,0.378,0.09,0.541-0.029c0.164-0.119,0.232-0.318,0.195-0.505c-0.428-2.078-1.071-5.191-1.071-5.191    s2.353-2.14,3.919-3.566c0.14-0.131,0.202-0.332,0.14-0.524s-0.23-0.319-0.42-0.341c-2.108-0.236-5.269-0.586-5.269-0.586    s-1.31-2.898-2.183-4.83c-0.082-0.173-0.254-0.294-0.456-0.294s-0.375,0.122-0.453,0.294C10.671,6.26,9.362,9.158,9.362,9.158z"></path></g></g></svg>
</label>

                    </div>
                    
                </div>
            </div>
        </template>
    </div>
</template>

<style scoped>

.container input {
  position: absolute;
  opacity: 0;
  cursor: pointer;
  height: 0;
  width: 0;
}

.container {
  display: block;
  position: relative;
  cursor: pointer;
  user-select: none;
}

.container svg {
  position: relative;
  top: 0;
  margin-top: 5px;
  right: 10px;
  height: 30px;
  width: 30px;
  transition: all 0.3s;
  fill: #666;
}



.container input:checked ~ svg {
  fill: #ffeb49;
}


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