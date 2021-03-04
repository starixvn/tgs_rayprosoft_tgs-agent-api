import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import {
  Group,
  Input,
  Box,
  Icon,
  Button,
  useToasts,
  useModalState,
  Dialog
} from "bumbag"
import { useForm, useFormContext, Controller } from "react-hook-form"
import "react-datepicker/dist/react-datepicker.css"
import { Trans, useLocale, useTrans } from "../i18n"
import DateTimeInput from "./DateTimeInput"
import { toDate } from "./toDate"
import min from "date-fns/min"
import max from "date-fns/max"
import isValid from "date-fns/isValid"
import addDays from "date-fns/addDays"
import subDays from "date-fns/subDays"
import differenceInDays from "date-fns/differenceInDays"
import { startOfNextDay } from "./startOfNextDay"
import { defaultFormatDate } from "./format"

export const DateInputRange = ({ dateFormat = "yyyy/MM/dd", ...props }) => (
  <DateTimeRangeInput
    dateFormat={dateFormat}
    showTimeInput={false}
    {...props}
  />
)

export const DateTimeRangeInput = ({
  startName = "start",
  endName = "end",
  minDate,
  maxDate,
  maxRangeDays,
  dateFormat,
  showTimeInput = true,
  required
}) => {
  let formMethods = useFormContext()

  let start = toDate(formMethods?.watch(startName))
  let end = toDate(formMethods?.watch(endName))

  let validate = Number.isInteger(maxRangeDays)
    ? {
        range: () => {
          let shallValidateRange =
            isValid(start) && isValid(end) && Number.isInteger(maxRangeDays)
          if (shallValidateRange) {
            return validateRange(maxRangeDays, start, end)
          }
        }
      }
    : undefined

  const toasts = useToasts()
  const hasRangeError = (name) => formMethods?.errors?.[name]?.type === "range"
  let rangeError = hasRangeError(startName) || hasRangeError(endName)
  let rangeErrorTitle = useRangeErrorTitle(maxRangeDays)
  useEffect(() => {
    if (rangeError) {
      toasts.add({
        title: rangeErrorTitle
      })
    }
  }, [rangeError])

  return (
    <>
      <Group>
        <DateTimeInput
          name={startName}
          dateFormat={dateFormat}
          showTimeInput={showTimeInput}
          minDate={minDate}
          maxDate={startMaxDate(maxDate, end)}
          required={required}
          validate={validate}
        />
        <Button isStatic>-</Button>
        <DateTimeInput
          name={endName}
          dateFormat={dateFormat}
          showTimeInput={showTimeInput}
          minDate={endMinDate(minDate, start)}
          maxDate={maxDate}
          required={required}
          validate={validate}
        />
      </Group>
    </>
  )
}

const useRangeErrorTitle = (maxRangeDays) =>
  useTrans({
    zh: `间隔需小于 ${maxRangeDays} 天`,
    en: `Interval should be less than ${maxRangeDays} days`
  })

const validateRange = (maxRangeDays, start, end) => {
  if (Number.isInteger(maxRangeDays) && isValid(start) && isValid(end)) {
    let includeStartDay = 1
    let days = differenceInDays(end, start) + includeStartDay
    let pass = days <= maxRangeDays
    return pass
  }
}

const startMaxDate = (maxDate, end) => {
  let validMaxDate = isValid(maxDate)
  let validEnd = isValid(end)
  if (validMaxDate && validEnd) {
    return min([maxDate, end])
  }

  if (validMaxDate) {
    return maxDate
  }

  return end
}

const endMinDate = (minDate, start) => {
  let validMinDate = isValid(minDate)
  let validStart = isValid(start)
  if (validMinDate && validStart) {
    return max([minDate, start])
  }

  if (validMinDate) {
    return minDate
  }

  return start
}

export default DateTimeRangeInput
