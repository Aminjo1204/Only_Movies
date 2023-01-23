<script setup>
import Footer from './components/Footer.vue';
import Navbar from './components/Navbar.vue';
</script>


<template>
    <div class="appContainer">
        <header>
            <Navbar></Navbar>
        </header>
        <main>
            <router-view />
        </main>
        <footer>
            <Footer></Footer>
        </footer>
    </div>
</template>

<style scoped>
.appContainer {
    max-width: 1280px;
    height: 100%;
    margin: 0 auto;
    font-weight: normal;
    padding: 1rem 2rem;
    background-color: white;
    border: 5px solid black;
    border-radius: 20px;
    display: flex;
    flex-direction: column;
    gap:1rem;
}

.appContainer main {
    flex-grow: 1;
    padding-bottom: 1rem;
    overflow-y: auto;
}

header {
    display: flex;
    justify-content: space-between;
    align-items: center;
}

footer {
    box-shadow: 0 0 50px rgba(0, 84, 103, 0.8);
    background-color: #fff;
    text-decoration: none;
}
</style>

<script>
export default {
    components: {
        Navbar,
        Footer,
    },
};
</script>