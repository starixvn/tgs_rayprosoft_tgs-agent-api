import format from "date-fns/format"
import isValid from "date-fns/isValid"
import { enUS, zhCN } from "date-fns/locale"
import { useLocale, Locales } from "../i18n"

export const Format = {
  Date: "yyyy/MM/dd",
  Time: "HH:mm:ss"
}

export const DateTimeFormat = `${Format.Date} ${Format.Time}`

export const useFormattedTimeString = (value) => {
  let date = new Date(value)
  return isValid(date) ? format(date, `HH:mm`) : ""
}

export default (date) =>
  isValid(date) ? format(date, `${Format.Date} ${Format.Time}`) : ""
