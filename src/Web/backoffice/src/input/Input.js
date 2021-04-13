import React, { useState, useEffect, useMemo, useRef, Children } from "react"
import { css, Global } from "@emotion/react"
import { useForm, useFormContext } from "react-hook-form"
import { Input as _Input, InputField as _InputField, Button } from "bumbag"
import { useTrans } from "../i18n"

export const Input = ({
    name,
    register,
    registerOptions = {},
    inputRef,
    validationText,
    errors,
    ...props
}) => {
    let formContext = useFormContext()
    register = register ?? formContext?.register

    let formProps = {}
    if (name && !inputRef && register) {
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
        inputRef = ref
    }

    let error = formContext?.formState?.errors[name]
    let state = error && "danger"

    return (
        <_Input state={state} inputRef={inputRef} {...formProps} {...props} />
    )
}

export const InputField = ({
    name,
    addonAfter,
    showEmptyFieldValueAddonAfter = false,
    register,
    registerOptions = {},
    inputRef,
    validationText,
    ...props
}) => {
    let formContext = useFormContext()
    register = register ?? formContext?.register

    let formProps = {}
    if (name && !inputRef && register) {
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
        inputRef = ref
    }

    let error = formContext?.formState?.errors[name]

    let state = error && "danger"
    validationText = validationText ?? error?.message

    addonAfter =
        addonAfter ??
        (showEmptyFieldValueAddonAfter && (
            <Button onClick={() => formContext?.setValue(name, "")}>X</Button>
        ))

    return (
        <_InputField
            inputRef={inputRef}
            state={state}
            validationText={validationText}
            addonAfter={addonAfter}
            {...formProps}
            {...props}
        />
    )
}

const useRequiredMessage = () =>
    useTrans({
        zh: "必填栏",
        en: "Required field"
    })

export default Input
