import { set } from "lodash-es"
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

const PromotionLocalization = "PromotionLocalization" //TODO: also remove env.html -> env.features.PromotionLocalization
export const Features = {
    PromotionLocalization
}

const listFeatures = () => {
    let features = Object.entries(Features ?? {})
    if (features.length > 0) {
        console.group("Features: ")
        features.map(([feature]) => {
            console.log(`${feature}`)
        })
        console.groupEnd()
    }
}

const enableFeatures = (...features) => {
    if (features.length > 0) {
        console.group("Enable: ")
        features.map((feature) => {
            let path = `env.features.${feature}`
            set(window, path, true)
            console.log(path)
        })
        console.groupEnd()
    }
}

window.featureX = {
    list: listFeatures,
    enable: {
        promotionLocalization: () => enableFeatures(PromotionLocalization)
    }
}

export const isFeatureEnabled = (feature = "") => {
    return window?.env?.features?.[feature] ?? false
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
    isFeatureEnabled,
    Features
}
