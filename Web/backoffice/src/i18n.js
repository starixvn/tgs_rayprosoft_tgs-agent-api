import React, { createContext, useContext, useState } from "react"
import { Text } from "bumbag"
import dayjs from "dayjs"
import numeral from "numeral"

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

  return <Store.Provider value={{ locale, set }}>{children}</Store.Provider>
}

export const useLocale = () => {
  let { locale } = useContext(Store)
  return locale ?? defaultLocale
}

export const useEnglish = () => {
  let { set } = useContext(Store)
  return () => set(Locales.en)
}

export const useChinese = () => {
  let { set } = useContext(Store)
  return () => set(Locales.zh)
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

export const FormattedNumber = ({ value }) => {
  return Number.isFinite(value) && <Text>{numeral(value).format("0,0")}</Text>
}
