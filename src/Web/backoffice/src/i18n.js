import React, { createContext, useContext, useState } from "react"
import { Text } from "bumbag"
import numeral from "numeral"
import { registerLocale, setDefaultLocale } from "react-datepicker"
import en from "date-fns/locale/en-US"
import zh from "date-fns/locale/zh-CN"
import { formatDateTime } from "./date/format"
registerLocale("en", en)
registerLocale("zh", zh)

const DefaultDateFormat = "yyyy/MM/dd"
const DefaultTimeFormat = "HH:mm:ss"

export const Locales = {
    en: "en",
    zh: "zh"
}

const defaultLocale = Locales.zh

const Store = createContext()

//TODO: default locale from cookie?
export const Provider = ({ value = Locales.zh, children }) => {
    let [locale, set] = useState(value)
    setDefaultLocale(locale)

    const setLocale = (locale) => {
        set(locale)
        setDefaultLocale(locale)
    }

    return (
        <Store.Provider value={{ locale, setLocale }}>
            {children}
        </Store.Provider>
    )
}

export const useLocale = () => {
    let { locale } = useContext(Store) ?? {}
    return locale ?? defaultLocale
}

export const useEnglish = () => {
    let { setLocale } = useContext(Store)
    return () => setLocale(Locales.en)
}

export const useChinese = () => {
    let { setLocale } = useContext(Store)
    return () => setLocale(Locales.zh)
}

export const useLangCode = () => {
    let enUS = "en-US"
    let Default = enUS
    let { locale } = useContext(Store) ?? {}
    switch (locale) {
        case Locales.en:
            return enUS
        case Locales.zh:
            return "zh-CN"
    }
    return Default
}

export const useTrans = (messages = {}) => {
    let locale = useLocale()
    return messages[locale]
}

export const Trans = ({ en, zh }) => {
    let locale = useLocale()
    switch (locale) {
        case Locales.en:
            return en ?? zh
        default:
            return zh ?? en
    }
}

export const FormattedDateTime = ({
    dateFormat = DefaultDateFormat,
    timeFormat = DefaultTimeFormat,
    value
}) => {
    let format = `${dateFormat} ${timeFormat}`
    return <Text>{formatDateTime(format, value)}</Text>
}

export const FormattedNumber = ({ value, format = "0,0" }) => {
    return (
        Number.isFinite(value) && <Text>{numeral(value).format(format)}</Text>
    )
}

export const getTrans = (messages = {}, locale = Locales.zh) =>
    Reflect.get(messages, locale) ?? ""
