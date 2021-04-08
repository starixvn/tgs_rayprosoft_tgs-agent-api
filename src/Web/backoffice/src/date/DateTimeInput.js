import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { Group, Input, Box, Icon, Button, FieldWrapper } from "bumbag"
import DatePicker from "react-datepicker"
import { useLocale } from "../i18n"
import { useForm, useFormContext, Controller } from "react-hook-form"
import format, { DateTimeFormat } from "./format"
import "./react-datepicker.css"

export const DateTimeInput = ({
    name,
    required,
    value,
    dateFormat = DateTimeFormat,
    showTimeInput = true,
    disabled,
    minDate,
    maxDate,
    validate,
    ...props
}) => {
    let locale = useLocale()
    let formMethods = useFormContext()
    let {
        control,
        formState: { errors }
    } = formMethods

    return (
        <Controller
            control={control}
            name={name}
            rules={{ required, validate }}
            render={({
                field: { onChange, onBlur, value, ref },
                fieldState: { invalid, isTouched, isDirty }
            }) => {
                if (disabled) {
                    return (
                        <Input
                            defaultValue={format(value)}
                            disabled={disabled}
                        />
                    )
                }

                let error = errors[name]
                let state = error && "danger"
                return (
                    <DatePicker
                        selected={value}
                        dateFormat={dateFormat}
                        onChange={onChange}
                        onBlur={onBlur}
                        minDate={minDate}
                        maxDate={maxDate}
                        locale={locale}
                        customInput={
                            <Input name={name} inputRef={ref} state={state} />
                        }
                        showTimeInput={showTimeInput}
                        timeInputLabel=""
                        {...props}
                    />
                )
            }}
        />
    )
}

export default DateTimeInput
