<script setup>
import axios from 'axios'; // npm install axios
</script>

<template>
    <div class="favouriteview">
        <h3>Favourite Movies</h3>
        <p class="mb-3">Use the search bar to find movies in the database.</p>
        <div class="formRow">
            <label>Title:</label>
            <input class="form-control" type="text" v-on:keyup="searchOnEnter($event)" v-model="title" />
            <button class="btn btn-outline-primary" v-on:click="searchForMovies()">Search</button>
        </div>
        <h5 v-if="showRecent">Recently added</h5>
        <h5 v-if="!showRecent">{{ movies.length }} Movies found.</h5>
        <div class="movielist" v-if="movies.length">
            <div class="movieitem" v-for="m in movies" v-bind:key="m.guid">
                <div class="movieposter">
                    <img v-bind:src="m.imageUrl" />
                </div>
                <div class="moviedata">
                    <div class="movieinfo">
                        <div class="movietitle">{{ m.title }} {{ m.year }}</div>
                        <div class="genres">{{ m.genres }}</div>
                        <div class="actors">{{ m.actors }}</div>
                    </div>
                    </div>
                    
                </div>
            </div>
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
    display: flex;
    flex-direction: column;
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

.moviedata {
    display: flex;
    flex-grow: 1;
    flex-direction: column;
}
.movieinfo {
    display: flex;
    flex-wrap: wrap;
    gap: 1em;
}
.comments {
    flex-grow: 1;
}
.movietitle {
    font-weight: bolder;
}

.deleteLink {
    color: red;
    cursor: pointer;
}
</style>

<script>
export default {
    data() {
        return {
            title: '',
            movies: [],
            newComments: {},
            showRecent: true,
        };
    },
    async mounted() {
        await this.searchForMovies();
    },
    methods: {
        async searchForMovies() {
            try {
                if (!this.title || this.title.length < 2) {
                    const response = await axios.get('movie/recent', { params: { count: 3 } });
                    this.movies = response.data;
                    this.showRecent = true;
                    return;
                }
                const response = await axios.get('movie/search', { params: { title: this.title } });
                this.movies = response.data;
                this.showRecent = false;
            } catch {
                alert('Error fetching data.');
            }
        },
        searchOnEnter(e) {
            if (e.key !== 'Enter') {
                return;
            }
            this.searchForMovies();
        },
        
    },
    computed: {
        authenticated() {
            return this.$store.state.userdata.username ? true : false;
        },
        isAdmin() {
            return this.$store.state.userdata.isAdmin == true;
        },
        userdata() {
            return this.$store.state.userdata;
        },
    },
};
</script>