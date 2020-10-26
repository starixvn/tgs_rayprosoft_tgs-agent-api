import _format from "date-fns/fp/format"
import isValid from "date-fns/fp/isValid"

export const Format = {
  Date: "yyyy/MM/dd",
  Time: "HH:mm:ss"
}

export const DateTimeFormat = `${Format.Date} ${Format.Time}`

const format = _format(`${Format.Date} ${Format.Time}`)

export default (date) => (isValid(date) ? format(date) : "")
