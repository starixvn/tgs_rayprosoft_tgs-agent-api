const httpProxy = require("http-proxy")
const apiProxy = httpProxy.createServer({ target: "http://localhost:32670" })
const globalApiProxy = httpProxy.createServer({
  target: "http://localhost:32666"
})

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
  devOptions: {
    port: 4000
  },
  buildOptions: {
    metaUrlPath: "snowpack"
  },
  routes: [
    {
      src: "/api/.*",
      dest: (req, res) => {
        return apiProxy.web(req, res)
      }
    },
    {
      src: "/global-api/.*",
      dest: (req, res) => {
        return globalApiProxy.web(req, res)
      }
    },
    { match: "routes", src: ".*", dest: "/index.html" }
  ]
}
