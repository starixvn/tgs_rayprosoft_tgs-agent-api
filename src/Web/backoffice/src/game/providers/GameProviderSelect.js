import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import Select from "../../select/Select"
import { Locales, Trans, useTrans } from "../../i18n"
import { useGameProviderList } from "./client"

export const GameProviderSelect = ({
    placeholder,
    valueField = "id",
    ...props
}) => {
    let defaultPlaceholder = useTrans({
        zh: "游戏厂商",
        en: "Game Provider"
    })
    placeholder = placeholder ?? defaultPlaceholder

    let { data, isLoading, error } = useGameProviderList({ showAll: true })
    let options = data?.items.map((item) => ({
        label: item.name,
        value: item[valueField]
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

export default GameProviderSelect
