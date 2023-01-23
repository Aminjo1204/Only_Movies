<template>
    <div class="footer-bottom">
        <p>Copyright &copy;2023 OnlyMovies, designed by <span>Team FAM</span></p>
    </div>
</template>
<style scoped>
.footer-content {
    display: flex;
    align-items: center;
    justify-content: center;
    flex-direction: column;
    text-align: center;
}

.footer-content h3 {
    padding-top: 15px;
}

.footer-content p {
    padding-bottom: 70px;
    width: 78%;
}

.pages {
    word-spacing: 50px;
    padding-bottom: 5px;
}

.pages a {
    padding-top: 20px;
    text-decoration: none;
}

.footer-bottom {
    background-color: rgb(151, 151, 151);
    text-align: center;
    padding: 10px;
}

.footer-bottom p {
    font-size: 18px;
    word-spacing: 2px;
    text-transform: capitalize;
}

.arrow {
    position: absolute;
    padding-bottom: 130px;
    left: 50%;
    transform: translate(-50%, -50%);
}

.arrow span {
    display: block;
    width: 15px;
    height: 15px;
    border-left: 3px solid rgb(1, 153, 191);
    border-top: 3px solid rgb(1, 153, 191);
    transform: rotate(45deg);
    animation: scroll 2s infinite;
}

.arrow span:nth-child(2) {
    animation-delay: -0.2s;
}

.arrow span:nth-child(3) {
    animation-delay: -0.4s;
}

@keyframes scroll {
    0% {
        opacity: 1;
        transform: rotate(45deg) translate(15px, 15px);
    }

    50% {
        opacity: 1;
    }

    100% {
        opacity: 0;
        transform: rotate(45deg) translate(-15px, -15px);
    }
}
</style>