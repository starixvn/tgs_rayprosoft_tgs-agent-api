import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import {
    Dialog,
    useModalState,
    Disclosure,
    FieldStack,
    Button,
    Icon
} from "bumbag"
import { faLanguage } from "@fortawesome/free-solid-svg-icons"
import FormDialog from "../form/FormDialog"
import clone from "clone-deep"
import { InputField } from "../input/Input"
import { ConfirmText } from "../button/ActionButtons"
import StaticButton from "../button/StaticButton"
import Backdrop from "./Backdrop"
import { sortBy } from "lodash-es"
import { useBrand } from "../merchant/brands/client"
import head from "../list/head"
import {
    Tabs,
    Tab,
    TabPanel as BaseTabPanel,
    TabList,
    useTabState
} from "../tab"
import { HtmlEditor } from "../text/editor/HtmlEditor"
import { CloseButtonFooter } from "../form/ConfirmButtonFooter"

export const LocalizationDialogMode = {
    List: "list",
    Tab: "tab"
}

const LocalizationFieldName = "localization"

const toLocalizationFieldName = (code, name) =>
    `${LocalizationFieldName}.${code}.${name}`

export const LocalizationDialogDisclosure = ({
    brandId = "",
    localizationField = LocalizationInputField,
    name = "",
    readOnly,
    disabled,
    mode = LocalizationDialogMode.List,
    width,
    maxWidth,
    height
}) => {
    let { isLoading, data: brand, error } = useBrand(brandId)
    let codes =
        sortBy(brand?.languages, ["brandCultureCode"])?.map(
            ({ brandCultureCode }) => brandCultureCode
        ) ?? []

    let dialog = useModalState()
    let { watch, setValue } = useFormContext()
    let fieldNames = codes.map((code) => toLocalizationFieldName(code, name))

    let values = watch(fieldNames)

    let record = {}
    codes.map((code, index) => {
        Reflect.set(record, code, values[index])
    })

    return (
        <>
            <Disclosure
                use={Button}
                isLoading={isLoading}
                disabled={isLoading || error}
                {...dialog}
            >
                <Icon icon={faLanguage} type="font-awesome" />
            </Disclosure>
            {dialog.visible && (
                <LocalizationDialog
                    readOnly={readOnly}
                    disabled={disabled}
                    localizationField={localizationField}
                    codes={codes}
                    record={record}
                    onChange={(data = {}) => {
                        let editable = !(readOnly || disabled)
                        if (editable) {
                            Object.entries(data).map(([code, value]) => {
                                let fieldName = toLocalizationFieldName(
                                    code,
                                    name
                                )
                                setValue(fieldName, value)
                            })
                        }
                    }}
                    dialog={dialog}
                    mode={mode}
                    width={width}
                    maxWidth={maxWidth}
                    height={height}
                />
            )}
        </>
    )
}

const LocalizationDialog = ({
    readOnly,
    disabled,
    localizationField,
    codes = [],
    record = {},
    onChange,
    dialog,
    mode,
    ...props
}) => {
    let title = <Icon icon={faLanguage} type="font-awesome" fontSize="500" />

    let editable = !(readOnly || disabled)
    return (
        <Backdrop {...dialog}>
            <FormDialog
                defaultValues={clone(record)}
                onSubmit={(data) => {
                    onChange?.(data)
                    dialog.hide()
                }}
                stopPropagation={true}
                dialog={dialog}
                title={title}
                submitText={<ConfirmText />}
                showActionButtons={editable}
                footer={
                    !editable && (
                        <CloseButtonFooter dialog={dialog} alignX="right" />
                    )
                }
                css={css`
                    .tox-fullscreen & {
                        top: 0;
                        left: 0;
                        transform: none;
                    }
                `}
                {...props}
            >
                <Body
                    readOnly={readOnly}
                    disabled={disabled}
                    localizationField={localizationField}
                    codes={codes}
                    mode={mode}
                />
            </FormDialog>
        </Backdrop>
    )
}

const LocalizationInputField = ({ code, disabled, readOnly, ...props }) => {
    return (
        <InputField
            name={code}
            disabled={disabled}
            readOnly={readOnly}
            addonBefore={<StaticButton>{code}</StaticButton>}
        />
    )
}

const Body = ({
    readOnly,
    disabled,
    mode = LocalizationDialogMode.List,
    localizationField,
    codes = []
}) => {
    switch (mode) {
        case LocalizationDialogMode.Tab:
            return (
                <TabsBody
                    readOnly={readOnly}
                    disabled={disabled}
                    localizationField={localizationField}
                    codes={codes}
                />
            )
        default:
            return (
                <ListBody
                    readOnly={readOnly}
                    disabled={disabled}
                    localizationField={localizationField}
                    codes={codes}
                />
            )
    }
}

const TabsBody = ({ readOnly, disabled, localizationField, codes = [] }) => {
    let selectedId = head(codes)

    let tab = useTabState({
        selectedId
    })

    return (
        <Tabs isFitted>
            <TabList use={TabList} {...tab}>
                {codes.map((code) => (
                    <Tab key={code} tabId={code} {...tab}>
                        {code}
                    </Tab>
                ))}
            </TabList>
            {codes.map((code, index) => (
                <TabPanel
                    key={code}
                    name={code}
                    localizationField={localizationField}
                    readOnly={readOnly}
                    disabled={disabled}
                    {...tab}
                />
            ))}
        </Tabs>
    )
}

const TabPanel = ({
    name,
    localizationField: LocalizationField,
    readOnly,
    disabled,
    ...props
}) => {
    let { selectedId } = props
    let isActive = selectedId === name
    return (
        <BaseTabPanel tabId={name} {...props}>
            {isActive && (
                <LocalizationField
                    code={name}
                    readOnly={readOnly}
                    disabled={disabled}
                />
            )}
        </BaseTabPanel>
    )
}

const ListBody = ({
    localizationField: LocalizationField,
    codes = [],
    readOnly,
    disabled
}) => {
    return (
        <FieldStack>
            {codes.map((code) => (
                <LocalizationField
                    key={code}
                    code={code}
                    readOnly={readOnly}
                    disabled={disabled}
                />
            ))}
        </FieldStack>
    )
}
