import React from "react"
import ReactDOM from "react-dom"
import App from "./App.jsx"
import { BrowserRouter } from "react-router-dom"
import { SWRConfig } from "swr"
import { Provider as BumbagProvider } from "bumbag"
import "./index.css"

const theme = {
  PageContent: {
    defaultProps: {
      breakpoint: "widescreen"
    }
  }
}

ReactDOM.render(
  <React.StrictMode>
    <BrowserRouter>
      <SWRConfig
        value={{
          revalidateOnFocus: false,
          shouldRetryOnError: false
        }}
      >
        <BumbagProvider theme={theme}>
          <App />
        </BumbagProvider>
      </SWRConfig>
    </BrowserRouter>
  </React.StrictMode>,
  document.getElementById("root")
)

// Hot Module Replacement (HMR) - Remove this snippet to remove HMR.
// Learn more: https://www.snowpack.dev/#hot-module-replacement
if (import.meta.hot) {
  import.meta.hot.accept()
}
