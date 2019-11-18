import Vue from "vue";
import axios from "axios";
import App from "./App.vue";
import router from "./router";
import VueAuthenticate from "vue-authenticate";

Vue.config.productionTip = false;

var config = {};
axios
  .get("/config/config.json")
  .then(function(resp) {
    config = resp.data;
  })
  .catch(function() {
    config = {
      baseURL: process.env.VUE_APP_API_BASE,
      authURL: process.env.VUE_APP_AUTH_ENDPOINT,
      authClientId: process.env.VUE_APP_CLIENT_ID
    };
  })
  .finally(function() {
    Vue.prototype.$config = config;

    Vue.use(VueAuthenticate, {
      tokenName: "access_token",
      storageType: "cookieStorage",
      providers: {
        oauth2: {
          authorizationEndpoint: config.authURL,
          clientId: config.authClientId,
          responseType: "token"
        }
      }
    });

    new Vue({
      router,
      render: h => h(App)
    }).$mount("#app");
  });
