const httpProxy = require("http-proxy")
const apiProxy = httpProxy.createServer({ target: "http://localhost:32667" })
const fileApiProxy = httpProxy.createServer({
    target: "http://localhost:32671"
})

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
    devOptions: {
        port: 3000
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
            src: "/fs/.*",
            dest: (req, res) => fileApiProxy.web(req, res)
        },
        { match: "routes", src: ".*", dest: "/index.html" }
    ]
}
