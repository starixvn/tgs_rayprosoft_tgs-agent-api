import setDay from "date-fns/setDay"
import format from "date-fns/format"
import isValid from "date-fns/isValid"
import { enUS, zhCN } from "date-fns/locale"
import { useLocale, Locales } from "../i18n"

const Sunday = 0
const Monday = 1
const Tuesday = 2
const Wednesday = 3
const Thursday = 4
const Friday = 5
const Saturday = 6

export const Day = {
  Sunday,
  Monday,
  Tuesday,
  Wednesday,
  Thursday,
  Friday,
  Saturday
}

const toLocale = (code) => {
  switch (code) {
    case Locales.en:
      return enUS
    default:
      return zhCN
  }
}

export const useFormattedDay = (value = Sunday) => {
  let code = useLocale()
  let locale = toLocale(code)
  let date = setDay(new Date(), value)
  return format(date, `EEEE`, { locale })
}

export const DaySelect = ({}) => {}

export const DaySelectField = ({}) => {}
