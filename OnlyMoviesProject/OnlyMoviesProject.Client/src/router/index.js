import { createRouter, createWebHistory } from 'vue-router'
import store from '../store.js'
import Login from '../views/Login.vue'
import HomeView from '../views/HomeView.vue'
import AddMovie from '../views/AddMovie.vue'
import CommentMovie from '../views/CommentMovie.vue'
import AboutUs from '../views/AboutUs.vue'
import Favourite from '../views/Favourite.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView
    },
    {
      path: '/addmovie',
      name: 'addmovie',
      component: AddMovie
    },
    {
      path: '/commentmovie',
      name: 'commentmovie',
      meta: { authorize: true },
      component: CommentMovie
    },    
    {
      path: '/login',
      name: 'login',
      component: Login
    },
    {
      path: '/aboutus',
      name: 'aboutus',
      component: AboutUs
    },
    {
      path: '/favouriteview',
      name: 'favouriteview',
     component: Favourite

    }   
  ]
});

router.beforeEach((to, from, next) => {
  const authenticated = store.state.userdata.username ? true : false;
  if (to.meta.authorize && !authenticated) {
    next("/login");
    return;
  }
  next();
  return;
});

export default router;