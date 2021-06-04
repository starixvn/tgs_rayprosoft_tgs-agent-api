import React from "react"
import ReactDOM from "react-dom"
import App from "./App"
import "./reset.css"
import "./index.css"
import { Provider as BumbagProvider, ToastManager } from "bumbag"
import { Provider as UserProvider } from "./user/context"
import { BrowserRouter as Router } from "react-router-dom"
import { Provider as I18nProvider } from "./i18n"
import {
    faTrash,
    faTrashAlt,
    faMinus,
    faEdit,
    faCog
} from "@fortawesome/free-solid-svg-icons"

const theme = {
    PageContent: {
        defaultProps: {
            breakpoint: "widescreen"
        }
    },
    Icon: {
        iconSets: [
            {
                icons: [faTrash, faTrashAlt, faMinus, faEdit, faCog],
                prefix: "solid-",
                type: "font-awesome"
            }
        ]
    }
}

ReactDOM.render(
    <React.StrictMode>
        <Router>
            <BumbagProvider theme={theme}>
                <UserProvider>
                    <I18nProvider>
                        <App />
                    </I18nProvider>
                </UserProvider>
                <ToastManager />
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
