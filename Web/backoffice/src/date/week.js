import React, { useState, useEffect, useMemo, useRef } from "react"
import setDay from "date-fns/setDay"
import format from "date-fns/format"
import isValid from "date-fns/isValid"
import { enUS, zhCN } from "date-fns/locale"
import { useLocale, Locales } from "../i18n"
import { Select, DropdownMenu, Button } from "bumbag"
import { useForm, useFormContext } from "react-hook-form"

const Sunday = "Sunday"
const Monday = "Monday"
const Tuesday = "Tuesday"
const Wednesday = "Wednesday"
const Thursday = "Thursday"
const Friday = "Friday"
const Saturday = "Saturday"

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

export const fromWeekDayString = (value) => {
  const map = {
    Sunday: 0,
    Monday: 1,
    Tuesday: 2,
    Wednesday: 3,
    Thursday: 4,
    Friday: 5,
    Saturday: 6
  }
  return map[value]
}

export const useFormattedDay = (value) => {
  value = typeof value === "string" ? fromWeekDayString(value) : value

  let code = useLocale()
  let locale = toLocale(code)
  let date = setDay(new Date(), value)
  return format(date, `EEEE`, { locale })
}

export const WeekDaySelect = ({ name, required, selectRef, ...props }) => {
  let { register, errors } = useFormContext()
  if (!selectRef) {
    selectRef = register({ required })
  }

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
      label: useFormattedDay(Sunday),
      value: Sunday
    }
  ]

  let error = errors?.[name]
  let state = error && "danger"
  return (
    <Select
      name={name}
      options={options}
      required={required}
      state={state}
      selectRef={selectRef}
      {...props}
    />
  )
}
