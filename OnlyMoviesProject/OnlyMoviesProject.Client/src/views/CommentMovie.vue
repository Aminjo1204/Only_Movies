<script setup>
import axios from 'axios'; // npm install axios
</script>

<template>
    <div class="commentMovieView">
        <h3>Comment movies</h3>
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
                    <div class="comments">
                        <div class="comment" v-for="f in m.feedbacks" v-bind:key="f.guid">
                            <div class="commentInfo">
                                <span v-if="isAdmin" class="deleteLink" v-on:click="deleteFeedback(f)">&#10006;</span>
                                {{ f.username }} @ {{ new Date(f.created).toLocaleString() }} wrote:
                            </div>
                            <div class="commentText">{{ f.text }}</div>
                        </div>
                    </div>
                    <div class="newComment">
                        <input type="text" class="form-control" v-on:keyup="addCommentOnEnter(m, $event)" v-model="m.newComment" placeholder="Post your comment." />
                        <button class="btn btn-outline-primary btn-sm" v-on:click="addComment(m)">Save</button>
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

.comment {
    display: flex;
    flex-wrap: wrap;
    gap: 0.2em;
}
.newComment {
    display: flex;
    gap: 0.5em;
}
.newComment input {
    flex-grow: 1;
    border: 1px solid hsl(180, 53%, 40%);
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
        async addComment(movie) {
            try {
                await axios.post('movie/comment', { movieGuid: movie.guid, userGuid: this.userdata.userGuid, text: movie.newComment });
            } catch {
                alert('Error saving the comment.');
            }
            movie.newComment = '';
            this.searchForMovies();
        },
        addCommentOnEnter(movie, e) {
            if (e.key !== 'Enter') {
                return;
            }
            this.addComment(movie);
        },
        async deleteFeedback(feedback) {
            try {
                await axios.delete(`movie/feedback/${feedback.guid}`);
            } catch {
                alert('Error deleting the comment.');
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