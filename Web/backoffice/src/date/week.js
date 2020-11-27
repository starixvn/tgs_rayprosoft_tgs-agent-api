import React, { useState, useEffect, useMemo, useRef } from "react"
import setDay from "date-fns/setDay"
import format from "date-fns/format"
import isValid from "date-fns/isValid"
import { enUS, zhCN } from "date-fns/locale"
import { useLocale, Locales } from "../i18n"
import { Select, DropdownMenu, Button } from "bumbag"
import { Field } from "../input/Field"

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

//TODO: use DropdownMenu or SelectMenu?
//TODO: value type string or number?
export const DaySelect = ({ ...props }) => {
  let options = [
    {
      label: useFormattedDay(Monday),
      value: Monday
    },
    {
      label: useFormattedDay(Tuesday),
      value: Tuesday
    },
    {
      label: useFormattedDay(Wednesday),
      value: Wednesday
    },
    {
      label: useFormattedDay(Thursday),
      value: Thursday
    },
    {
      label: useFormattedDay(Friday),
      value: Friday
    },
    {
      label: useFormattedDay(Saturday),
      value: Saturday
    },
    {
      label: useFormattedDay(Saturday),
      value: Saturday
    }
  ]

  return <Select options={options} {...props} />
}

export const DaySelectField = ({ name, ...props }) => {
  return (
    <Field name={name} {...props}>
      {({ field }) => <DaySelect name={name} {...field} />}
    </Field>
  )
}
