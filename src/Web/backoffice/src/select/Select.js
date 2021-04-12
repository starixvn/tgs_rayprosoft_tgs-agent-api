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

    let formProps = {}
    if (name && !selectRef && register) {
        let { ref, ...rest } = register(name, {
            required: props.required,
            min: props.min,
            max: props.max,
            minLength: props.minLength,
            maxLength: props.maxLength,
            validate: props.validate,
            ...registerOptions
        })
        formProps = rest
        selectRef = ref
    }

    state = state ?? (formContext?.formState?.errors?.[name] && "danger")

    return (
        <_Select
            selectRef={selectRef}
            state={state}
            {...formProps}
            {...props}
            onChange={(event) => {
                formProps?.onChange?.(event)
                props?.onChange?.(event)
            }}
        />
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
    let formProps = {}
    if (name && !selectRef && register) {
        let { ref, ...rest } = register(name, {
            required: props.required,
            min: props.min,
            max: props.max,
            minLength: props.minLength,
            maxLength: props.maxLength,
            validate: props.validate,
            ...registerOptions
        })
        formProps = rest
        selectRef = ref
    }

    let error = formContext?.formState?.errors[name]
    let state = error && "danger"
    validationText = validationText ?? error?.message

    return (
        <_SelectField
            selectRef={selectRef}
            {...formProps}
            state={state}
            validationText={validationText}
            {...props}
        />
    )
}

export default Select
