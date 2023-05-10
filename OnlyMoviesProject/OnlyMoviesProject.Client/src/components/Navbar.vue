<script setup>
import axios from 'axios';
</script>

<template>
    <router-link to="/"><img class="logo" src="../assets/logo-farbe.jpeg" /></router-link>
    <nav>
        <ul class="nav_links">
            <router-link to="/addmovie">
                <li>Search &amp; add movies</li>
            </router-link>
            <router-link to="/commentmovie">
                <li>Comment movies</li>
            </router-link>
            <router-link to="/favouriteview">
                <li>Favourite</li>
            </router-link>
            <router-link to="/aboutus">
                <li>Über uns</li>
            </router-link>
            <router-link to="/login" v-if="!authenticated">
                <li>
                    <a>Login</a>
                </li>
            </router-link>
            <a href="" v-on:click="logout()"><li v-if="authenticated">Logout {{ userdata.username }}</li></a>
        </ul>
    </nav>
</template>


<style scoped>
header .logo {
    width: 220px;
    cursor: pointer;
}

.nav_links {
    margin-right: 100px;
}

.nav_links li {
    display: inline-block;
    padding: 0px 10px;
}

.nav_links li a {
    transition: all 0.3s ease 0s;
}

.nav_links li a:hover {
    color: rgba(0, 185, 232, 0.8);
    font-size: 18px;
    text-decoration: none;
}
</style>

<script>
export default {
    setup() {},
    computed: {
        authenticated() {
            return this.$store.state.userdata.username ? true : false;
        },
        userdata() {
            return this.$store.state.userdata;
        },
    },
    methods: {
        logout() {
                delete axios.defaults.headers.common['Authorization'];
                this.$store.commit('authenticate', null);
        }
    }
};
</script>