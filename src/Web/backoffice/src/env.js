/**
 * env =
 * {
 *  mode: "production" | "development",
 *  loginButtonColor: "#ffffff" | "#212121",
 *  loginButtonBackground: "#1e417b" | "#f7f7fa",
 *  headerBackground: "#1e417b" | "#f7f7fa"
 *  headerButtonColor: "#ffffff" | "#212121",
 *  headerButtonBackground: "transparent" | ""
 * }
 *
 * Note: env declared in public/env.html
 *
 * ref:
 * https://caddyserver.com/docs/modules/http.handlers.templates
 */

const Development = "development"

const Production = "production"

export const isDev = () => window?.env?.mode === Development

export const isProduction = () => window?.env?.mode === Production

export const getLoginButtonColor = () =>
    window?.env?.loginButtonColor || "#ffffff"

export const getLoginButtonBackground = () =>
    window?.env?.loginButtonBackground || "#1e417b"

export const getHeaderBackground = () =>
    window?.env?.headerBackground || "#1e417b"

export const getHeaderButtonColor = () =>
    window?.env?.headerButtonColor || "#ffffff"

export const getHeaderButtonBackground = () =>
    window?.env?.loginButtonBackground || "transparent"

//TODO: init feature object
window.enableFeature = (...features) => {
    console.log("enableFeature", features)
}

export const isFeatureEnabled = (feature = "") => {
    return window?.features?.[feature] ?? false
}

export default {
    Development,
    Production,
    isDev,
    isProduction,
    getLoginButtonColor,
    getLoginButtonBackground,
    getHeaderBackground,
    getHeaderButtonColor,
    getHeaderButtonBackground,
    isFeatureEnabled
}
