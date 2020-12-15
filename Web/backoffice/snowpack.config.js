/** @type {import("snowpack").SnowpackUserConfig } */
module.exports = {
  mount: {
    public: { url: "/", static: true },
    static: { url: "/", static: true, resolve: false },
    src: { url: "/dist" }
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
    port: 3000
  },
  buildOptions: {
    metaDir: "snowpack"
  },
  proxy: {
    "/fs/file/uploadFileS3": "http://localhost:32671/api/file/uploadFileS3",
    "/api": "http://localhost:32667/api"
  },
  alias: {
    /* ... */
  }
}
