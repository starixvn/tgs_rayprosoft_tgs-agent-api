/**
 * env =
 * {
 *  mode: "production" | "development",
 *  loginButtonPalette: "primary" | "",
 *  headerBackground: "" | "#f7f7fa"
 * }
 */

const Development = "development"

const Production = "production"

export const isDev = () => window?.env?.mode === Development

export const isProduction = () => window?.env?.mode === Production

export const getLoginButtonPalette = () =>
    window?.env?.loginButtonPalette || "default"

export const getHeaderBackground = () => window?.env?.headerBackground

export default {
    Development,
    Production,
    isDev,
    isProduction,
    getLoginButtonPalette,
    getHeaderBackground
}
