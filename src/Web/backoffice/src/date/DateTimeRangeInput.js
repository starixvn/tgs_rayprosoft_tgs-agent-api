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
    Dialog,
    FieldWrapper,
    FieldStack
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

export const DateInputRange = ({ dateFormat = "yyyy/MM/dd", ...props }) => (
    <DateTimeRangeInput
        dateFormat={dateFormat}
        showTimeInput={false}
        {...props}
    />
)

export const DateInputRangeField = ({
    label,
    startProps,
    endProps,
    ...props
}) => {
    let startFieldName = startProps?.name
    let endFieldName = endProps?.name
    let formMethods = useFormContext()
    return (
        <FieldWrapper
            label={
                <FieldStack orientation="horizontal" alignY="center">
                    {label}
                    {formMethods && (startFieldName || endFieldName) && (
                        <Button.Close
                            size="small"
                            onClick={() => {
                                formMethods.setValue(startFieldName, "")
                                formMethods.setValue(endFieldName, "")
                            }}
                        />
                    )}
                </FieldStack>
            }
        >
            <DateInputRange
                startProps={startProps}
                endProps={endProps}
                {...props}
            />
        </FieldWrapper>
    )
}

export const DateTimeRangeInput = ({
    startProps,
    endProps,
    minDate,
    maxDate,
    maxRangeDays,
    dateFormat,
    showTimeInput = true,
    required,
    disabled
} = {}) => {
    let startName = startProps?.name ?? "start"
    let endName = endProps?.name ?? "end"

    let formMethods = useFormContext()
    let start = toDate(formMethods?.watch(startName))
    let end = toDate(formMethods?.watch(endName))

    let validate = Number.isInteger(maxRangeDays)
        ? {
              range: () => {
                  let shallValidateRange =
                      isValid(start) &&
                      isValid(end) &&
                      Number.isInteger(maxRangeDays)
                  if (shallValidateRange) {
                      return validateRange(maxRangeDays, start, end)
                  }
              }
          }
        : undefined

    const toasts = useToasts()
    const hasRangeError = (name) =>
        formMethods?.formState?.errors?.[name]?.type === "range"
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
                    maxDate={startMaxDate([startProps?.maxDate, maxDate, end])}
                    required={required}
                    disabled={disabled}
                    validate={validate}
                />
                <Button isStatic tabIndex="-1">
                    -
                </Button>
                <DateTimeInput
                    name={endName}
                    dateFormat={dateFormat}
                    showTimeInput={showTimeInput}
                    minDate={endMinDate([endProps?.minDate, minDate, start])}
                    maxDate={maxDate}
                    required={required}
                    disabled={disabled}
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

const startMaxDate = (dates = []) => {
    dates = dates.filter(isValid)
    return min(dates)

    /*
    let validMaxDate = isValid(maxDate)
    let validEnd = isValid(end)
    if (validMaxDate && validEnd) {
        return min([maxDate, end])
    }

    if (validMaxDate) {
        return maxDate
    }

    return end
    */
}

const endMinDate = (dates = []) => {
    dates = dates.filter(isValid)
    return max(dates)

    /*
    let validMinDate = isValid(minDate)
    let validStart = isValid(start)
    if (validMinDate && validStart) {
        return max([minDate, start])
    }

    if (validMinDate) {
        return minDate
    }

    return start
    */
}

export default DateTimeRangeInput
