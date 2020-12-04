/** @type {import("snowpack").SnowpackUserConfig } */
module.exports = {
  mount: {
    public: "/",
    src: "/dist"
  },
  plugins: [
    "@snowpack/plugin-babel",
    "@snowpack/plugin-react-refresh",
    "@snowpack/plugin-dotenv"
  ],
  install: [
    /* ... */
  ],
  installOptions: {
    /* ... */
  },
  devOptions: {
    port: 4000
  },
  buildOptions: {
    /* ... */
  },
  proxy: {
    "/api": "http://localhost:32670/api"
  },
  alias: {
    /* ... */
  }
}
