import React from "react"
import ReactDOM from "react-dom"
import App from "./App"
import "./reset.css"
import "./index.css"
import { Provider as BumbagProvider } from "bumbag"
import { Provider as UserProvider } from "./user/context"
import { BrowserRouter as Router } from "react-router-dom"

const theme = {
  PageContent: {
    defaultProps: {
      breakpoint: "desktop"
    }
  }
}

ReactDOM.render(
  <React.StrictMode>
    <Router>
      <BumbagProvider theme={theme}>
        <UserProvider>
          <App />
        </UserProvider>
      </BumbagProvider>
    </Router>
  </React.StrictMode>,
  document.getElementById("root")
)

// Hot Module Replacement (HMR) - Remove this snippet to remove HMR.
// Learn more: https://www.snowpack.dev/#hot-module-replacement
if (import.meta.hot) {
  import.meta.hot.accept()
}
