import parseISO from "date-fns/parseISO"
import format from "date-fns/format"
import isValid from "date-fns/isValid"
import { useLocale, Locales } from "../i18n"

const Format = {
  Date: "yyyy/MM/dd",
  Time: "HH:mm:ss"
}

export const DateTimeFormat = `${Format.Date} ${Format.Time}`

export const useFormattedTimeString = (value) => {
  let date = new Date(value)
  return isValid(date) ? format(date, `HH:mm`) : ""
}

const DefaultDateTimeFormat = `${Format.Date} ${Format.Time}`
export const formatDateTime = (dateFormat = DefaultDateTimeFormat, value) => {
  if (!value) {
    return ""
  }

  let date = typeof value == "string" ? parseISO(value) : value
  return isValid(date) ? format(date, dateFormat) : ""
}

export const defaultFormatDateTime = (value) =>
  formatDateTime(DefaultDateTimeFormat, value)

export const defaultFormatDate = (value) => formatDateTime(Format.Date, value)

export const formatDayMonthYear = (value) => formatDateTime("dd/MM/yyyy", value)

export default defaultFormatDateTime
