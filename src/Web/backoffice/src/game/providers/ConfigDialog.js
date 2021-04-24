import React, { useState, useEffect, useMemo, useRef } from "react"
import { css, Global } from "@emotion/react"
import {
    Table,
    Set,
    Heading,
    Text,
    useModalState,
    Icon,
    FieldStack,
    Box,
    Group,
    Button
} from "bumbag"
import { useTrans } from "../../i18n"
import { InputField } from "../../input/Input"
import { SelectField } from "../../select/Select"
import { Switch, SwitchField } from "../../switch/Switch"
import FormDialog from "../../form/FormDialog"
import Disclosure from "../../dialog/Disclosure"
import { faCog, faDesktop, faMobile } from "@fortawesome/free-solid-svg-icons"

const ConfigDisclosure = ({ ...props }) => {
    return (
        <Disclosure {...props}>
            <Icon icon={faCog} type="font-awesome" />
        </Disclosure>
    )
}

const ConfigDialog = ({ dialog, ...props }) => {
    let title = useTrans({
        zh: `配置`,
        en: `Configuration`
    })
    return (
        <FormDialog
            dialog={dialog}
            title={title}
            hideBackdrop={true}
            minWidth="500px"
        >
            <FieldStack>
                <NameInputField />
                <DescriptionInputField />
                <FieldStack orientation="horizontal">
                    <DefaultSwitchField />
                    <ConfigurationStatusSelectField />
                </FieldStack>
                <InputField name="clientId" label="Client Id" />
                <InputField name="secretKey" label="Secret Key" />

                <InputField name="desktopEndpoint" label="Desktop Endpoint" />
                <InputField name="mobileEndpoint" label="Mobile Endpoint" />
                {/* TODO: json textarea editor */}
                <InputField
                    name="configurationData"
                    label="Configuration Data"
                />
            </FieldStack>
        </FormDialog>
    )
}

const NameInputField = ({ name = "name", ...props }) => {
    let label = useTrans({
        zh: "名称",
        en: "Name"
    })

    return <InputField name={name} label={label} {...props} />
}

const DescriptionInputField = ({ name = "description", ...props }) => {
    let label = useTrans({
        zh: "说明",
        en: "Description"
    })

    return <InputField name={name} label={label} {...props} />
}

const DefaultSwitchField = ({ name = "isDefault" }) => {
    let label = useTrans({
        zh: "预设",
        en: "Default"
    })

    return <SwitchField name={name} label={label} />
}

const Active = "Active"
const ActiveTrans = {
    zh: "开启",
    en: "Enabled"
}

const Inactive = "Inactive"
const InactiveTrans = {
    zh: "关闭",
    en: Inactive
}

const Maintenance = "Maintenance"
const MaintenanceTrans = {
    zh: "维护",
    en: Maintenance
}
const ConfigurationStatusSelectField = ({ name = "status", ...props }) => {
    let label = useTrans({
        zh: "状态",
        en: "Status"
    })
    let placeholder = useTrans({
        zh: "选择状态",
        en: "Select Status"
    })

    let options = [
        {
            label: useTrans(ActiveTrans),
            value: Active
        },
        {
            label: useTrans(InactiveTrans),
            value: Inactive
        },
        {
            label: useTrans(MaintenanceTrans),
            value: Maintenance
        }
    ]
    return (
        <SelectField
            name={name}
            label={label}
            placeholder={placeholder}
            options={options}
            {...props}
        />
    )
}
