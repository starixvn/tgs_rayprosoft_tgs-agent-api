import React, { useState, useEffect, useMemo, useRef } from "react"
import { useLocale, Locales } from "../i18n"
import { Select, SelectField, DropdownMenu, Button, Icon } from "bumbag"
import { useFormikContext } from "formik"
import { Field } from "../input/Field"
import startOfDay from "date-fns/startOfDay"
import endOfDay from "date-fns/endOfDay"
import isBefore from "date-fns/isBefore"
import isAfter from "date-fns/isAfter"
import format from "date-fns/format"
import addMinutes from "date-fns/addMinutes"
import setHours from "date-fns/setHours"
import setMinutes from "date-fns/setMinutes"
import { faClock } from "@fortawesome/free-regular-svg-icons"

export const TimeIcon = () => <Icon icon={faClock} type="font-awesome" />

/*
export const TimeSelect = ({ min, max, ...props }) => {
  let options = rangeOptions()
  return <Select options={options} {...props} />
}
*/

export const TimeSelectField = ({
  name,
  min,
  max,
  addonBefore = (
    <Button isStatic>
      <TimeIcon />
    </Button>
  ),
  ...props
}) => {
  /* TODO: use min and max range options?
  let minDate = toDate(min, startOfDay(new Date()))
  let maxDate = toDate(max, endOfDay(new Date()))
  let options = rangeOptions(minDate, maxDate)
    */
  let minDate = toDate(min, startOfDay(new Date()))
  let maxDate = toDate(max, endOfDay(new Date()))
  let options = rangeOptions()
  return (
    <Field
      name={name}
      validate={(value) => {
        if (value && minDate && !isAfter(toDate(value), minDate)) {
          return "min"
        }

        if (value && maxDate && !isBefore(toDate(value), maxDate)) {
          return "max"
        }
      }}
      {...props}
    >
      {({ field, form, state, validationText }) => {
        // validationText={validationText}
        return (
          <SelectField.Formik
            options={options}
            addonBefore={addonBefore}
            state={state}
            {...field}
          />
        )
      }}
    </Field>
  )
}

const toDate = (value = "", defaultValue) => {
  if (value) {
    let [hours, minutes] = value.split(":")
    value = setHours(new Date(), parseInt(hours))
    value = setMinutes(value, parseInt(minutes))
    return value
  }
  return defaultValue
}

const rangeOptions = (
  start = startOfDay(new Date()),
  end = endOfDay(new Date()),
  minutes = 30
) => {
  let options = []
  while (isBefore(start, end)) {
    let time = format(start, "HH:mm")
    options.push({
      label: time,
      value: time
    })

    start = addMinutes(start, minutes)
  }

  return options
}
