import React, { createContext, useContext, useState } from "react"
import { Text } from "bumbag"
//TODO: use date-fns instead
import dayjs from "dayjs"
import numeral from "numeral"
import { registerLocale, setDefaultLocale } from "react-datepicker"
import en from "date-fns/locale/en-US"
import zh from "date-fns/locale/zh-CN"
registerLocale("en", en)
registerLocale("zh", zh)

const DefaultDateFormat = "YYYY/MM/DD"
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
    <Store.Provider value={{ locale, setLocale }}>{children}</Store.Provider>
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
  let date = dayjs(value)
  if (!date.isValid()) {
    //empty
    return ""
  }

  let formatted = `${date.format(dateFormat)} ${date.format(timeFormat)}`
  return <Text>{formatted}</Text>
}

export const FormattedNumber = ({ value, format = "0,0" }) => {
  return Number.isFinite(value) && <Text>{numeral(value).format(format)}</Text>
}

export const getTrans = (messages = {}, locale = Locales.zh) =>
  Reflect.get(messages, locale) ?? ""
