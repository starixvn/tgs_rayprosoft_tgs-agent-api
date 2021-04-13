import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import { useGameProvider } from "./client"
import Select from "../../select/Select"
import { useTrans } from "../../i18n"

export const GameProviderLanguageSelect = ({
    placeholder,
    gameProviderId,
    ...props
}) => {
    let defaultPlaceholder = useTrans({
        zh: "选择游戏商语言",
        en: "Select Game Provider Language"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let { isLoading, data, error } = useGameProvider(gameProviderId)
    let options = data?.languages?.map(({ cultureCode, ...lang }) => ({
        label: cultureCode,
        value: cultureCode
    }))

    return (
        <Select
            placeholder={placeholder}
            isLoading={isLoading}
            options={options}
            {...props}
        />
    )
}
