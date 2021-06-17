import React, { useState, useEffect, useMemo, useRef, Children } from "react"
import { css, Global } from "@emotion/react"
import { useForm, useFormContext } from "react-hook-form"
import {
    FieldStack,
    FieldWrapper,
    Box,
    Set,
    Textarea as _Textarea,
    TextareaField as BaseTextareaField
} from "bumbag"
import { LocalizationDialogDisclosure } from "../dialog/LocalizationDialog"

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
            css={css`
                & textarea {
                    resize: ${resize};
                }
            `}
            {...formProps}
            {...props}
            onChange={(event) => {
                formProps?.onChange?.(event)
                props?.onChange?.(event)
            }}
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
    addonAfter,
    brandId,
    showLocalizationAddonAfter = false,
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

    addonAfter =
        addonAfter ??
        (showLocalizationAddonAfter && (
            <LocalizationDialogDisclosure
                brandId={brandId}
                localizationField={LocalizationTextareaField}
                name={name}
            />
        ))

    return (
        <Set alignY="top" isFilled>
            <BaseTextareaField
                textareaRef={textareaRef}
                state={state}
                validationText={validationText}
                flex="1"
                css={css`
                    & textarea {
                        resize: ${resize};
                    }
                `}
                {...formProps}
                {...props}
            />
            {addonAfter}
        </Set>
    )
}

const LocalizationTextareaField = ({ code, ...props }) => {
    return (
        <Set isFilled alignY="center">
            <Box>{code}</Box>
            <TextareaField name={code} />
        </Set>
    )
}

export default Textarea
