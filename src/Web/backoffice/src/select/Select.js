import React, { useState, useEffect, useMemo, useRef, Children } from "react"
import { css, Global } from "@emotion/react"
import { useForm, useFormContext } from "react-hook-form"
import { Spinner, Select as _Select, SelectField as _SelectField } from "bumbag"

export const Select = ({
    name,
    register,
    registerOptions = {},
    selectRef,
    state,
    ...props
}) => {
    let formContext = useFormContext()
    register = register ?? formContext?.register
    if (!selectRef && register) {
        selectRef = register({
            required: props.required,
            min: props.min,
            max: props.max,
            minLength: props.minLength,
            maxLength: props.maxLength,
            validate: props.validate,
            ...registerOptions
        })
    }

    state = state ?? (formContext?.errors?.[name] && "danger")

    return (
        <_Select name={name} selectRef={selectRef} state={state} {...props} />
    )
}

export const SelectField = ({
    name,
    register,
    registerOptions = {},
    selectRef,
    validationText,
    ...props
}) => {
    let formContext = useFormContext()

    register = register ?? formContext?.register
    if (!selectRef && register) {
        selectRef = register({
            required: {
                value: props.required,
                message: props.requiredMessage
            },
            min: {
                value: props.min,
                message: props.minMessage
            },
            max: {
                value: props.max,
                message: props.maxMessage
            },
            minLength: {
                value: props.minLength,
                message: props.minLengthMessage
            },
            maxLength: {
                value: props.maxLength,
                message: props.maxLengthMessage
            },
            validate: props.validate,
            ...registerOptions
        })
    }

    let error = formContext?.errors[name]
    let state = error && "danger"
    validationText = validationText ?? error?.message

    return (
        <_SelectField
            name={name}
            selectRef={selectRef}
            state={state}
            validationText={validationText}
            {...props}
        />
    )
}

export default Select
