<script setup>
import axios from 'axios';
</script>

<template>
    <div class="loginView">
        <div v-if="!authenticated">
            <p class="mb-3">
                Melde dich mit einen Zugangsdaten an, um Filme einfügen und kommentieren zu können. Tipp: Ein Adminuser ist <em>admin</em> mit dem Passwort <em>1111</em>. Ein normaler User ist
                <em>user</em> mit dem Passwort <em>1111</em>. Der Admin kann Kommentare löschen, normale User dürfen nur Kommentare posten.
            </p>
            <div class="formRow">
                <label>Username:</label>
                <input class="form-control" v-model="model.username" type="text" />
            </div>
            <div class="formRow">
                <label>Password:</label>
                <input class="form-control" v-model="model.password" type="password" />
            </div>
            <div>
                <button class="btn btn-outline-primary" v-on:click="sendLoginData()">Submit</button>
            </div>
        </div>
        <div v-if="authenticated">User {{ userdata.username }} logged in.</div>
    </div>
</template>

<style scoped>
.loginView {
    padding: 2em 3em;
    border: 2px solid hsl(180, 53%, 80%);
    border-radius: 1em;
    max-width: 50em;
    margin: 2em auto;
    display: flex;
    flex-direction: column;
    gap: 1em;
}
.formRow {
    display: flex;
    align-items: center;
    margin-bottom: 1em;
}

.formRow label {
    display: block;
    flex: 0 0 6em;
}
.formRow input {
    flex-grow: 1;
}
</style>

<script>
export default {
    setup() {},
    data() {
        return {
            message: '',
            model: {
                username: 'user',
                password: '1111',
            },
        };
    },
    mounted() {
        this.message = '';
    },
    methods: {
        async sendLoginData() {
            try {
                const userdata = (await axios.post('user/login', this.model)).data;
                axios.defaults.headers.common['Authorization'] = `Bearer ${userdata.token}`;
                this.$store.commit('authenticate', userdata);
                this.message = `User ${userdata.username} logged in.`;
            } catch (e) {
                if (e.response.status == 401) {
                    alert('Login failed. Invalid credentials.');
                }
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