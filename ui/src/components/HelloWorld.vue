<template>
  <div class="hello">
    <h1>{{ msg }}</h1>
    <div class="authentication-status" v-if="isAuthenticated">
      You are successfully authenticated
      <ul>
        <li>{{ values }}</li>
      </ul>
    </div>

    <div class="authentication-status" v-if="!isAuthenticated">
      <button @click="authLogin()">Login</button>
    </div>
    <div class="authentication-status" v-if="isAuthenticated">
      <button @click="authLogout()">Logout</button>
    </div>
  </div>
</template>

<script>
export default {
  name: "HelloWorld",
  props: {
    msg: String
  },
  data: function() {
    var values = [];
    var this_ = this;
    if (this.$auth.isAuthenticated()) {
      this.$http
        .get(`${process.env.VUE_APP_API_BASE}/api/values`)
        .then(function(resp) {
          this_.values = resp.data;
        });
    }

    return {
      isAuthenticated: this.$auth.isAuthenticated(),
      values
    };
  },
  methods: {
    authLogin: function() {
      if (this.$auth.isAuthenticated()) {
        this.$auth.logout();
      }

      this.response = null;

      var this_ = this;
      this.$auth
        .authenticate("oauth2")
        .then(function() {
          this_.isAuthenticated = this_.$auth.isAuthenticated();

          this_.$http
            .get(`${process.env.VUE_APP_API_BASE}/api/values`)
            .then(function(resp) {
              this_.values = resp.data;
            });
        })
        .catch(function(err) {
          this_.isAuthenticated = this_.$auth.isAuthenticated();
          this_.response = err;
        });
    }
  }
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
h3 {
  margin: 40px 0 0;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}
</style>
