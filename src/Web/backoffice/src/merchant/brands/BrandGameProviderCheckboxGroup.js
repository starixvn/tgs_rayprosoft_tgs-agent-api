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
import { useFormContext, useFieldArray, Controller } from "react-hook-form"
import { isAllPropChecked } from "../../checkbox/isAllChecked"
import { Trans, useTrans } from "../../i18n"
import { Checkbox, CheckboxField } from "../../checkbox"
import Disclosure, { ConfigIconDisclosure } from "../../dialog/Disclosure"
import FormDialog from "../../form/FormDialog"
import { Input, InputField } from "../../input/Input"
import { Select, SelectField } from "../../select/Select"
import { Switch, SwitchField } from "../../switch/Switch"
import Backdrop from "../../dialog/Backdrop"
import {
    faDesktop,
    faMobile,
    faLanguage,
    faCog
} from "@fortawesome/free-solid-svg-icons"
import { CloseText, ConfirmText } from "../../button/ActionButtons"
import { GameProviderConfigSelect } from "../../game/providers/configs/GameProviderConfigSelect"
import { GameProviderLanguageSelect } from "../../game/providers/GameProviderLanguageSelect"

export const BrandGameProviderCheckboxGroupTable = ({
    name = "gameProviders",
    fieldArrayContext,
    width = "100%",
    disabled = false,
    ...props
}) => {
    let { control, setValue, watch, register } = useFormContext()

    const { fields } = fieldArrayContext

    let selection = watch(name)
    let allChecked = isAllPropChecked(selection)

    return (
        <Set orientation="vertical" width={width}>
            <Table width={width}>
                <Table.Head>
                    <Table.Row>
                        <Table.HeadCell textAlign="center">
                            <Box></Box>
                        </Table.HeadCell>
                        <Table.HeadCell textAlign="center">
                            <Trans zh="游戏厂商" en="Game Provider" />
                        </Table.HeadCell>
                        <Table.HeadCell textAlign="center">
                            <Trans zh="设定" en="Configuration" />
                        </Table.HeadCell>
                    </Table.Row>
                </Table.Head>
                <Table.Body>
                    {fields.map((field, index) => (
                        <Row
                            key={field._id}
                            name={name}
                            disabled={disabled}
                            field={field}
                            index={index}
                        />
                    ))}
                </Table.Body>
            </Table>
        </Set>
    )
}

const Row = ({ name, disabled, field, index }) => {
    let formContext = useFormContext()
    let { register, setValue, control, getValues } = formContext

    let configDialog = useModalState()
    let defaultLangDialog = useModalState()
    let desktopQueryStringDialog = useModalState()
    let mobileQueryStringDialog = useModalState()

    let checkedFieldName = toCheckedFieldName(name, field, index)
    let {
        ref: checkedRef,
        onChange: onCheckedChange,
        ...checkedFormProps
    } = formContext?.register?.(checkedFieldName)

    let editable = !disabled
    return (
        <Table.Row>
            <Table.Cell textAlign="center">
                <Box>
                    {disabled && (
                        <>
                            <Checkbox
                                display="none"
                                checked={field.checked}
                                checkboxRef={checkedRef}
                                {...checkedFormProps}
                            />
                            <Checkbox checked={field.checked} disabled />
                        </>
                    )}
                    {editable && (
                        <Checkbox
                            checkboxRef={checkedRef}
                            defaultChecked={field.checked}
                            {...checkedFormProps}
                            onChange={(event) => {
                                onCheckedChange(event)
                                let checked = event.target.checked

                                setValue(checkedFieldName, checked)
                                if (checked) {
                                    configDialog.show()
                                }
                            }}
                        />
                    )}
                    <Input
                        type="hidden"
                        name={`${name}.${index}.id`}
                        value={field.id}
                    />
                    <Input
                        type="hidden"
                        name={`${name}.${index}.gameProviderId`}
                        value={field.gameProviderId}
                    />
                </Box>
            </Table.Cell>
            <Table.Cell textAlign="center">{field.name}</Table.Cell>
            <Table.Cell textAlign="center">
                <Set orientation="horizontal" alignX="center">
                    <Controller
                        control={control}
                        name={configFieldName(name, field, index)}
                        defaultValue={field.configurationId}
                        shouldUnregister={true}
                        rules={{
                            validate: (value) => {
                                let isChecked = getValues(
                                    toCheckedFieldName(name, field, index)
                                )
                                if (isChecked && !value) {
                                    return "required"
                                }
                            }
                        }}
                        render={({
                            field: {
                                onChange,
                                onBlur,
                                value,
                                name: configName,
                                ref
                            },
                            fieldState: { invalid, isTouched, isDirty }
                        }) => {
                            let palette = invalid ? "danger" : "default"
                            let variant = invalid ? "outlined" : "default"
                            return (
                                <>
                                    <ConfigIconDisclosure
                                        variant={variant}
                                        palette={palette}
                                        {...configDialog}
                                    />
                                    {configDialog.visible && (
                                        <ConfigDialog
                                            gameProvider={field}
                                            configurationId={value}
                                            disabled={disabled}
                                            dialog={configDialog}
                                            onChange={(configId) => {
                                                setValue(configName, configId, {
                                                    shouldValidate: true
                                                })
                                            }}
                                        />
                                    )}
                                </>
                            )
                        }}
                    />
                    {/* TODO: only show default lang select when B2B? */}
                    {/* <Controller
                        control={control}
                        name={langFieldName(name, field, index)}
                        defaultValue={field.defaultLanguageCode ?? ""}
                        shouldUnregister={true}
                        render={({
                            field: {
                                onChange,
                                onBlur,
                                value,
                                name: fieldName,
                                ref
                            },
                            fieldState: { invalid, isTouched, isDirty }
                        }) => {
                            return (
                                <>
                                    <Disclosure {...defaultLangDialog}>
                                        <Icon
                                            icon={faLanguage}
                                            type="font-awesome"
                                        />
                                    </Disclosure>
                                    {defaultLangDialog.visible && (
                                        <DefaultLangDialog
                                            defaultLanguageCode={value}
                                            gameProvider={field}
                                            disabled={disabled}
                                            dialog={defaultLangDialog}
                                            onChange={(code) => {
                                                setValue(fieldName, code)
                                            }}
                                        />
                                    )}
                                </>
                            )
                        }}
                    /> */}
                    <Controller
                        control={control}
                        name={queryStringFieldName(
                            name,
                            field,
                            index,
                            QueryStringType.Desktop
                        )}
                        defaultValue={field.desktopQueryString ?? ""}
                        shouldUnregister={true}
                        render={({
                            field: {
                                onChange,
                                onBlur,
                                value,
                                name: fieldName,
                                ref
                            },
                            fieldState: { invalid, isTouched, isDirty }
                        }) => {
                            return (
                                <>
                                    <Disclosure {...desktopQueryStringDialog}>
                                        <Icon
                                            icon={faDesktop}
                                            type="font-awesome"
                                        />
                                    </Disclosure>
                                    {desktopQueryStringDialog.visible && (
                                        <QueryStringDialog
                                            type={QueryStringType.Desktop}
                                            value={value}
                                            disabled={disabled}
                                            dialog={desktopQueryStringDialog}
                                            onChange={(queryString) => {
                                                setValue(fieldName, queryString)
                                            }}
                                        />
                                    )}
                                </>
                            )
                        }}
                    />
                    <Controller
                        control={control}
                        name={queryStringFieldName(
                            name,
                            field,
                            index,
                            QueryStringType.Mobile
                        )}
                        defaultValue={field.mobileQueryString ?? ""}
                        shouldUnregister={true}
                        render={({
                            field: {
                                onChange,
                                onBlur,
                                value,
                                name: fieldName,
                                ref
                            },
                            fieldState: { invalid, isTouched, isDirty }
                        }) => {
                            return (
                                <>
                                    <Disclosure {...mobileQueryStringDialog}>
                                        <Icon
                                            icon={faMobile}
                                            type="font-awesome"
                                        />
                                    </Disclosure>
                                    {mobileQueryStringDialog.visible && (
                                        <QueryStringDialog
                                            type={QueryStringType.Mobile}
                                            value={value}
                                            disabled={disabled}
                                            dialog={mobileQueryStringDialog}
                                            onChange={(queryString) => {
                                                setValue(fieldName, queryString)
                                            }}
                                        />
                                    )}
                                </>
                            )
                        }}
                    />
                </Set>
            </Table.Cell>
        </Table.Row>
    )
}

const QueryStringType = {
    Desktop: "desktopQueryString",
    Mobile: "mobileQueryString"
}

const useQueryStringTitle = (type = QueryStringType.Desktop) => {
    let desktopTitle = useTrans({
        zh: `桌面搜索参数`,
        en: `Desktop Query String`
    })

    let mobileTitle = useTrans({
        zh: `移动搜索参数`,
        en: `Mobile Query String`
    })

    switch (type) {
        case QueryStringType.Desktop:
            return desktopTitle
        case QueryStringType.Mobile:
            return mobileTitle
    }
}

const QueryStringDialog = ({ type, value, disabled, dialog, onChange }) => {
    let title = useQueryStringTitle(type)
    return (
        <Backdrop {...dialog}>
            <FormDialog
                stopPropagation={true}
                onSubmit={(data) => {
                    onChange(data.url)
                    dialog.hide()
                }}
                submitText={disabled ? <CloseText /> : <ConfirmText />}
                dialog={dialog}
                title={title}
                hideBackdrop={true}
                minWidth="500px"
            >
                <InputField
                    label="Query String"
                    name="url"
                    defaultValue={value}
                    disabled={disabled}
                />
            </FormDialog>
        </Backdrop>
    )
}

const ConfigDialog = ({
    gameProvider,
    configurationId,
    disabled,
    dialog,
    onChange
}) => {
    let title = useTrans({
        zh: `游戏商配置`,
        en: `Game Provider Configuration`
    })

    return (
        <Backdrop {...dialog}>
            <FormDialog
                stopPropagation={true}
                defaultValues={{
                    id: configurationId
                }}
                onSubmit={({ id, ...data }) => {
                    if (!disabled) {
                        onChange(id)
                    }
                    dialog.hide()
                }}
                submitText={disabled ? <CloseText /> : <ConfirmText />}
                dialog={dialog}
                title={title}
                hideBackdrop={true}
                minWidth="max-content"
            >
                <GameProviderConfigSelect
                    name="id"
                    gameProviderId={gameProvider?.gameProviderId}
                    defaultValue={configurationId}
                    required={!disabled}
                    disabled={disabled}
                />
            </FormDialog>
        </Backdrop>
    )
}

const DefaultLangDialog = ({
    gameProvider,
    defaultLanguageCode,
    disabled,
    dialog,
    onChange
}) => {
    let title = useTrans({
        zh: `游戏商预设语言`,
        en: `Game Provider Default Language`
    })

    return (
        <Backdrop {...dialog}>
            <FormDialog
                stopPropagation={true}
                onSubmit={({ code }) => {
                    onChange(code)
                    dialog.hide()
                }}
                submitText={disabled ? <CloseText /> : <ConfirmText />}
                dialog={dialog}
                title={title}
                hideBackdrop={true}
                minWidth="max-content"
            >
                <GameProviderLanguageSelect
                    name="code"
                    gameProviderId={gameProvider?.gameProviderId}
                    defaultValue={defaultLanguageCode}
                    disabled={disabled}
                />
            </FormDialog>
        </Backdrop>
    )
}

const toCheckedFieldName = (name, field, index) => `${name}.${index}.checked`

const queryStringFieldName = (name, field, index, type) =>
    `${name}.${index}.${type}`

const configFieldName = (name, field, index) =>
    `${name}.${index}.configurationId`

const langFieldName = (name, field, index) =>
    `${name}.${index}.defaultLanguageCode`
