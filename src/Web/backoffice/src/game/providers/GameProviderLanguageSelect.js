import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useGameProvider } from "./client"
import Select from "../../select/Select"
import { useTrans } from "../../i18n"
import { Controller, useFormContext } from "react-hook-form"

export const GameProviderLanguageSelect = ({
    name,
    gameProviderId,
    defaultValue,
    disabled,
    placeholder,
    placeholderDisabled,
    ...props
}) => {
    let formMethods = useFormContext()
    let { data, isLoading, error } = useGameProvider(gameProviderId)

    let defaultPlaceholder = useTrans({
        zh: "选择游戏商语言",
        en: "Select Game Provider Language"
    })

    if (name && formMethods && isLoading) {
        return (
            <Controller
                name={name}
                control={formMethods.control}
                defaultValue={props.defaultValue}
                render={() => <Select isLoading options={[]} />}
            />
        )
    }

    let options =
        data?.languages?.map(({ cultureCode, ...lang }) => ({
            label: cultureCode,
            value: cultureCode
        })) ?? []

    placeholder = placeholder ?? defaultPlaceholder
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

    return (
        <Select
            name={name}
            options={options}
            isLoading={isLoading}
            disabled={disabled}
            {...props}
        />
    )
}
