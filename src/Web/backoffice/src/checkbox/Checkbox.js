import React, { useState, useEffect, useMemo, useRef, Children } from "react"
import { css, Global } from "@emotion/react"
import { useForm, useFormContext, Controller } from "react-hook-form"
import { Checkbox as _Checkbox, CheckboxField as _CheckboxField } from "bumbag"
import get from "lodash-es/get"

export const Checkbox = ({
    name,
    state,
    checkboxRef,
    register,
    registerOptions = {},
    ...props
}) => {
    let formContext = useFormContext()
    register = register ?? formContext?.register

    let formProps = {}
    if (name && !checkboxRef && register) {
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
        checkboxRef = ref
    }

    let error = get(formContext?.formState?.errors ?? {}, name)
    state = state ?? (error && "danger")

    return (
        <_Checkbox
            state={state}
            checkboxRef={checkboxRef}
            {...formProps}
            {...props}
        />
    )
}

export const CheckboxField = ({
    name,
    checkboxRef,
    validationText,
    register,
    registerOptions = {},
    ...props
}) => {
    let formContext = useFormContext()
    register = register ?? formContext?.register

    let formProps = {}
    if (name && !checkboxRef && register) {
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
        checkboxRef = ref
    }

    let error = formContext?.formState?.errors[name]
    let state = error && "danger"
    validationText = validationText ?? error?.message

    return (
        <_CheckboxField
            checkboxRef={checkboxRef}
            state={state}
            validationText={validationText}
            {...formProps}
            {...props}
        />
    )
}

export default Checkbox
