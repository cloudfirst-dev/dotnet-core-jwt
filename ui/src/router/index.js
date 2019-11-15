import Vue from "vue";
import VueRouter from "vue-router";
import Home from "../views/Home.vue";
import VueAuthenticate from "vue-authenticate";
import VueAxios from "vue-axios";
import axios from "axios";

Vue.use(VueAxios, axios);
Vue.use(VueRouter);
Vue.use(VueAuthenticate, {
  tokenName: "access_token",
  storageType: "cookieStorage",
  providers: {
    oauth2: {
      authorizationEndpoint: process.env.VUE_APP_AUTH_ENDPOINT,
      clientId: process.env.VUE_APP_CLIENT_ID,
      responseType: "token"
    }
  }
});

const routes = [
  {
    path: "/",
    name: "home",
    component: Home
  },
  {
    path: "/about",
    name: "about",
    // route level code-splitting
    // this generates a separate chunk (about.[hash].js) for this route
    // which is lazy-loaded when the route is visited.
    component: () =>
      import(/* webpackChunkName: "about" */ "../views/About.vue")
  }
];

const router = new VueRouter({
  mode: "history",
  base: process.env.BASE_URL,
  routes
});

export default router;
