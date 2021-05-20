import React, { useState, useEffect, useMemo } from "react"
import {
    css,
    Stack,
    Heading,
    Box,
    Button,
    Select,
    SelectMenu,
    DropdownMenu,
    Switch
} from "bumbag"
import { useTrans, Trans } from "../../i18n"
import { SelectField } from "../../select/Select"

const Active = "Active"
const Inactive = "Inactive"
const Archived = "Archived"
const Maintenance = "Maintenance"
const Testing = "Testing"

export const GameStatus = {
    Active,
    Inactive
}

export const GameStatusSelectField = ({ placeholder, ...props }) => {
    let defaultPlaceholder = useTrans({
        zh: "Game Status",
        en: "Game Status"
    })
    placeholder = placeholder ?? defaultPlaceholder
    let options = [
        {
            label: Active,
            value: Active
        },
        {
            label: Inactive,
            value: Inactive
        },
        {
            label: Archived,
            value: Archived
        },
        {
            label: Maintenance,
            value: Maintenance
        },
        {
            label: Testing,
            value: Testing
        }
    ]
    return (
        <SelectField placeholder={placeholder} options={options} {...props} />
    )
}
