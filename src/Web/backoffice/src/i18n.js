import React, { createContext, useContext, useState } from "react"
import { Text, DropdownMenu, Button } from "bumbag"
import numeral from "numeral"
import { registerLocale, setDefaultLocale } from "react-datepicker"
import en from "date-fns/locale/en-US"
import zh from "date-fns/locale/zh-CN"
import {
    formatDateTime,
    formatHourMinuteSecondDayMonthYear,
    HourMinuteSecondDayMonthYear
} from "./date/format"
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

const Key = "lang"
//1. lang from localStorage
//2. lang from navigator.language
export const getAcceptLang = () => {
    let lang
    try {
        lang = localStorage.getItem(Key)
        if (lang) {
            return lang
        }
    } catch (error) {
        //ignore
    }

    lang = Object.values(Locales).reduce((result, lang) => {
        if (result) {
            return result
        }

        if (navigator.language.startsWith(lang)) {
            return lang
        }
    }, lang)

    return lang ?? Locales.en
}

export const getAcceptLangCulture = () => {
    let lang = getAcceptLang()
    switch (lang) {
        case Locales.zh:
            return "zh-CN"
        default:
            return "en-US"
    }
}

export const Provider = ({ value = getAcceptLang(), children }) => {
    let [locale, set] = useState(value)
    setDefaultLocale(locale)

    const setLocale = (locale) => {
        set(locale)
        setDefaultLocale(locale)
        try {
            localStorage.setItem(Key, locale)
        } catch (error) {
            //ignore
        }
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

export const FormattedDate = ({
    format = DefaultDateFormat,
    value,
    placeholder = ""
}) => {
    let formatted = formatDateTime(format, value)
    return <Text>{formatted ? formatted : placeholder}</Text>
}

export const FormattedDateTime = ({
    dateFormat = DefaultDateFormat,
    timeFormat = DefaultTimeFormat,
    value,
    placeholder = ""
}) => {
    let format = `${dateFormat} ${timeFormat}`
    let formatted = formatDateTime(format, value)
    return <Text>{formatted ? formatted : placeholder}</Text>
}

export const FormatHourMinuteSecondDayMonthYear = ({
    value,
    placeholder = ""
}) => {
    let formatted = formatHourMinuteSecondDayMonthYear(value)
    return formatted ? formatted : placeholder
}

let MaxDigits = 20
export const FormattedNumber = ({
    value,
    digits,
    format = "0,0",
    postfix = ""
}) => {
    if (!Number.isFinite(value)) {
        return null
    }

    if (Number.isInteger(digits) && digits >= 0 && digits <= MaxDigits) {
        value = value.toFixed(digits)
    }
    return (
        <Text>
            {numeral(value).format(format)}
            {postfix}
        </Text>
    )
}

export const FormattedNumberDecimal = ({
    value,
    digits = 2,
    format = "0,0.00",
    postfix = ""
}) => {
    if (!Number.isFinite(value)) {
        return null
    }

    if (Number.isInteger(digits) && digits >= 0 && digits <= MaxDigits) {
        value = value.toFixed(digits)
    }
    return (
        <Text>
            {numeral(value).format(format)}
            {postfix}
        </Text>
    )
}

export const getTrans = (messages = {}, locale = Locales.zh) =>
    Reflect.get(messages, locale) ?? ""

export const LangDropdownMenu = ({ ...props }) => {
    let locale = useLocale()
    let chinese = "中文"
    let english = "English"

    let useEn = useEnglish()
    let useZh = useChinese()

    return (
        <DropdownMenu
            menu={
                <>
                    <DropdownMenu.Item onClick={useZh}>
                        {chinese}
                    </DropdownMenu.Item>
                    <DropdownMenu.Item onClick={useEn}>
                        {english}
                    </DropdownMenu.Item>
                </>
            }
        >
            <Button variant="ghost" iconAfter="chevron-down">
                {locale === Locales.zh && chinese}
                {locale === Locales.en && english}
            </Button>
        </DropdownMenu>
    )
}
