import React, { useState, useEffect, useMemo, useRef, Children } from "react"
import { css, Global } from "@emotion/react"
import { useForm, useFormContext } from "react-hook-form"
import {
    Spinner,
    Stack,
    Flex,
    Box,
    Card,
    Button,
    Text,
    Input,
    Checkbox,
    FieldStack,
    FieldWrapper,
    SelectField,
    Table,
    Select,
    Group,
    Set,
    useClassName,
    Textarea as _Textarea,
    TextareaField as _TextareaField
} from "bumbag"

export const Textarea = ({
    name,
    register,
    registerOptions = {},
    textareaRef,
    resize = "none",
    ...props
}) => {
    let formContext = useFormContext()
    register = register ?? formContext?.register

    let formProps = {}
    if (name && !textareaRef && register) {
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
        textareaRef = ref
    }

    let error = formContext?.formState?.errors[name]
    let state = error && "danger"

    return (
        <_Textarea
            textareaRef={textareaRef}
            state={state}
            {...formProps}
            {...props}
            css={css`
                & textarea {
                    resize: ${resize};
                }
            `}
        />
    )
}

export const TextareaField = ({
    name,
    register,
    registerOptions = {},
    textareaRef,
    resize = "none",
    validationText,
    ...props
}) => {
    let formContext = useFormContext()
    register = register ?? formContext?.register

    let formProps = {}
    if (name && !textareaRef && register) {
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
        textareaRef = ref
    }

    let error = formContext?.formState?.errors[name]
    let state = error && "danger"
    validationText = validationText ?? error?.message

    return (
        <_TextareaField
            textareaRef={textareaRef}
            state={state}
            validationText={validationText}
            css={css`
                & textarea {
                    resize: ${resize};
                }
            `}
            {...formProps}
            {...props}
        />
    )
}

export default Textarea
