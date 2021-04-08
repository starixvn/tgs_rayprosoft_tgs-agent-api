import React, { useState, useEffect, useMemo, useRef, Children } from "react"
import { css, Global } from "@emotion/react"
import { useForm, useFormContext, Controller } from "react-hook-form"
import { Spinner, Switch as _Switch, SwitchField as _SwitchField } from "bumbag"

export const Switch = ({
    name,
    switchRef,
    register,
    registerOptions = {},
    ...props
}) => {
    let formContext = useFormContext()
    register = register ?? formContext?.register

    let formProps = {}
    if (name && !switchRef && register) {
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
        switchRef = ref
    }

    let error = formContext?.formState?.errors[name]
    let state = error && "danger"

    return (
        <_Switch
            switchRef={switchRef}
            state={state}
            {...formProps}
            {...props}
        />
    )
}

export const SwitchField = ({
    name,
    switchRef,
    validationText,
    register,
    registerOptions = {},
    ...props
}) => {
    let formContext = useFormContext()
    register = register ?? formContext?.register

    let formProps = {}
    if (name && !switchRef && register) {
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
        switchRef = ref
    }

    let error = formContext?.formState?.errors[name]
    let state = error && "danger"
    validationText = validationText ?? error?.message

    return (
        <_SwitchField
            switchRef={switchRef}
            state={state}
            validationText={validationText}
            {...formProps}
            {...props}
        />
    )
}

export default Switch
