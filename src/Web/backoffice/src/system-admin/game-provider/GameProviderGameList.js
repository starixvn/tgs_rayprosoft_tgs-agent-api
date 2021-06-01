import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import {
    Stack,
    Heading,
    Box,
    Button,
    Text,
    Group,
    Set,
    Flex,
    FieldStack,
    useModalState,
    Dialog,
    Card,
    Level
} from "bumbag"
import { useIsDisplayMenuByRoute } from "../../user/context"
import Disclosure, {
    AddDisclosure,
    EditDisclosure
} from "../../dialog/Disclosure"
import {
    useForm,
    useFormContext,
    useFieldArray,
    Controller,
    FormProvider
} from "react-hook-form"
import FormDialog from "../../form/FormDialog"
import { Trans, useLocale, useTrans } from "../../i18n"
import Backdrop from "../../dialog/Backdrop"
import LoadingBackdrop from "../../dialog/LoadingBackdrop"
import AlertDialog from "../../dialog/Alert"
import clone from "clone-deep"
import { pageCount, Table, toApiParams, useTable } from "../../table"
import textColumn from "../../table/column/textColumn"
import { InputField, Input } from "../../input/Input"
import {
    AddGameProviderGameDisclosure,
    GameProviderGameDisclosure
} from "./GameProviderGame"
import { BatchAddGameProviderGameListDisclosure } from "./BatchAddGameProviderGameList"
import SearchButton from "../../button/SearchButton"
import { useGameProviderGameList } from "../../game/games/client"
import dateTimeColumn from "../../table/column/dateTime"
import { View, Update } from "./menu"
import { CloseButtonFooter } from "../../form/ConfirmButtonFooter"

export const GameProviderGameListDisclosure = ({ gameProviderId }) => {
    let dialog = useModalState()

    let isDisplay = useIsDisplayMenuByRoute([View])
    if (!isDisplay) {
        return null
    }

    return (
        <>
            <Disclosure {...dialog}>
                <Trans zh="游戏列表" en="Game List" />
            </Disclosure>
            {dialog.visible && (
                <GameListDialog
                    gameProviderId={gameProviderId}
                    dialog={dialog}
                />
            )}
        </>
    )
}

const GameListDialog = ({ gameProviderId, dialog, ...props }) => {
    let title = useTrans({
        zh: "游戏列表",
        en: "Game List"
    })

    let { state, setState, setPageIndex, setSortOrder } = useTable({
        gameProviderId,
        code: "",
        id: ""
    })
    let { pageIndex, pageSize, code, id } = state
    let { isLoading, data, error, mutate, deleteCache } =
        useGameProviderGameList({ ...toApiParams(state), sortby: "id" })

    let items = data?.items ?? []
    let columns = useColumns({ items, mutate })

    if (error) {
        return <AlertDialog {...error} onClickClose={dialog.hide} />
    }

    return (
        <Dialog.Modal
            title={title}
            {...dialog}
            footer={<CloseButtonFooter alignX="right" dialog={dialog} />}
            showCloseButton
            hideOnClickOutside={false}
            placement="top"
            width="80vw"
            maxWidth="80vw"
        >
            <FieldStack>
                <ActionBar
                    gameProviderId={gameProviderId}
                    id={id}
                    code={code}
                    isLoading={isLoading}
                    search={(data) => {
                        setState({
                            pageIndex: 0,
                            ...data
                        })
                        mutate()
                    }}
                    mutate={mutate}
                />
                <Table
                    columns={columns}
                    data={items}
                    pageCount={pageCount(state.pageSize, data?.totalCount)}
                    setPageIndex={setPageIndex}
                    setSortOrder={setSortOrder}
                    isLoading={isLoading}
                />
            </FieldStack>
        </Dialog.Modal>
    )
}

const ActionBar = ({ gameProviderId, code, id, search, mutate, isLoading }) => {
    let formMethods = useForm({
        defaultValues: { code, id }
    })

    return (
        <Level>
            <FormProvider {...formMethods}>
                <form onSubmit={formMethods.handleSubmit(search)}>
                    <Set>
                        {/* <InputField
                            name="id"
                            showEmptyFieldValueAddonAfter
                            placeholder="游戏ID"
                        /> */}
                        <InputField
                            name="code"
                            showEmptyFieldValueAddonAfter
                            placeholder="Code"
                        />
                        <SearchButton
                            isLoading={isLoading}
                            disabled={isLoading}
                        />
                    </Set>
                </form>
            </FormProvider>
            <Set orientation="horizontal">
                <AddGameProviderGameDisclosure
                    gameProviderId={gameProviderId}
                    onSubmitted={mutate}
                />

                <BatchAddGameProviderGameListDisclosure
                    gameProviderId={gameProviderId}
                    onSubmitted={mutate}
                />
            </Set>
        </Level>
    )
}

const useColumns = ({ items = [], mutate }) => {
    let hasUpdateAuth = useIsDisplayMenuByRoute([Update])
    return useMemo(() => {
        let columns = [
            textColumn({
                accessor: "id",
                Header: <Trans zh="游戏ID" en="Game ID" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "code",
                Header: <Trans zh="游戏代码" en="Code" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "name",
                Header: <Trans zh="游戏名称" en="Name" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "status",
                Header: <Trans zh="状态" en="Status" />,
                disableSortBy: true,
                Cell: ({ value = "" }) => value
            }),
            dateTimeColumn({
                accessor: "updatedOn",
                Header: <Trans zh="最新更新" en="Updated On" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "updatedByUsername",
                Header: <Trans zh="更改者" en="Updated By" />,
                disableSortBy: true
            })
        ]

        if (hasUpdateAuth) {
            columns.push(
                textColumn({
                    id: "actions",
                    accessor: (row) => row,
                    Header: <Trans zh="操作" en="Actions" />,
                    disableSortBy: true,
                    Cell: ({ value: record }) => {
                        return (
                            <Set orientation="horizontal" alignX="center">
                                <GameProviderGameDisclosure
                                    record={record}
                                    onSubmitted={mutate}
                                />
                            </Set>
                        )
                    }
                })
            )
        }

        return columns
    }, [JSON.stringify(items)])
}
