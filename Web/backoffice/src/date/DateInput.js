import React, { useState, useEffect, useMemo, useRef } from "react"
import { Group, Input, Box, Icon, Button, css, FieldWrapper } from "bumbag"
import DatePicker from "react-datepicker"
import { useLocale } from "../i18n"
import { useForm, useFormContext, Controller } from "react-hook-form"
import format, { DateTimeFormat } from "./format"

export const HookFormDateInput = ({
  name,
  required,
  value,
  disabled,
  minDate,
  maxDate,
  ...props
}) => {
  let locale = useLocale()
  let { register, control, errors, ...formMethods } = useFormContext()

  return (
    <Controller
      control={control}
      name={name}
      rules={{ required }}
      render={(
        { onChange, onBlur, value, ref, ...props },
        { invalid, isTouched, isDirty }
      ) => {
        if (disabled) {
          return <Input defaultValue={format(value)} disabled={disabled} />
        }

        let error = errors[name]
        let state = error && "danger"
        return (
          <DatePicker
            selected={value}
            dateFormat={DateTimeFormat}
            onChange={onChange}
            onBlur={onBlur}
            minDate={minDate}
            maxDate={maxDate}
            locale={locale}
            customInput={<Input name={name} inputRef={ref} state={state} />}
            showTimeInput
            timeInputLabel=""
          />
        )
      }}
    />
  )
}

export default HookFormDateInput
