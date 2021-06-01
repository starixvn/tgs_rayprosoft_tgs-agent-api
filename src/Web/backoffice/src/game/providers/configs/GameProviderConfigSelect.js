import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import Select from "../../../select/Select"
import { Locales, Trans, useTrans } from "../../../i18n"
import { useGameProviderConfigList } from "./client"

export const GameProviderConfigSelect = ({
    placeholder,
    gameProviderId,
    ...props
}) => {
    let defaultPlaceholder = useTrans({
        zh: "游戏商配置",
        en: "Game Provider Configuration"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let { data, isLoading, error } = useGameProviderConfigList({
        gameProviderId,
        showAll: true,
        sortby: "name"
    })
    let options = data?.items?.map(({ name, id, ...item }) => ({
        label: name,
        value: id
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
