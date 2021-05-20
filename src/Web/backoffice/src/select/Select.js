import React, { useState, useEffect, useMemo, useRef, Children } from "react"
import { css, Global } from "@emotion/react"
import { useForm, useFormContext, Controller } from "react-hook-form"
import { Spinner, Select as _Select, SelectField as _SelectField } from "bumbag"

export const Select = ({
    name,
    register,
    registerOptions = {},
    selectRef,
    state,
    options = [],
    placeholder,
    placeholderDisabled = false,
    ...props
}) => {
    let formContext = useFormContext()
    register = register ?? formContext?.register

    let formProps = {}
    let autoRegisterFormField = name && !selectRef && register
    if (autoRegisterFormField) {
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

    // let loadingOptions = props.isLoading && (!options || options.length === 0)
    if (autoRegisterFormField && props.isLoading) {
        return (
            <Controller
                name={name}
                control={formContext.control}
                defaultValue={props?.defaultValue}
                render={() => {
                    return <Select isLoading options={[]} />
                }}
            />
        )
    }

    if (typeof placeholder === "string") {
        options = [
            {
                label: placeholder,
                value: "",
                disabled: placeholderDisabled
            },
            ...options
        ]
    }

    state = state ?? (formContext?.formState?.errors?.[name] && "danger")

    return (
        <_Select
            options={options}
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
    options = [],
    placeholder,
    placeholderDisabled = false,
    ...props
}) => {
    let formContext = useFormContext()

    register = register ?? formContext?.register
    let formProps = {}
    let autoRegisterFormField = name && !selectRef && register
    if (autoRegisterFormField) {
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

    // let loadingOptions = props.isLoading && (!options || options.length === 0)
    if (autoRegisterFormField && props.isLoading) {
        return (
            <Controller
                name={name}
                control={formContext.control}
                defaultValue={props?.defaultValue}
                render={() => {
                    return <Select isLoading options={[]} />
                }}
            />
        )
    }

    if (typeof placeholder === "string") {
        options = [
            {
                label: placeholder,
                value: "",
                disabled: placeholderDisabled
            },
            ...options
        ]
    }

    let error = formContext?.formState?.errors[name]
    let state = error && "danger"
    validationText = validationText ?? error?.message

    return (
        <_SelectField
            options={options}
            selectRef={selectRef}
            {...formProps}
            state={state}
            validationText={validationText}
            {...props}
        />
    )
}

export default Select
