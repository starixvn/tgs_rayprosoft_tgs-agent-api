import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import { Heading, Stack, Button, Set, Text, Box, Level } from "bumbag"
import PageContent from "../../page/Content"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute
} from "../../user/context"
import { Code, Update } from "./menu"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { Trans, useTrans } from "../../i18n"
import { SearchButton } from "../../button/SearchButton"
import equals from "fast-deep-equal"
import { FormProvider, useForm } from "react-hook-form"
import { InputField } from "../../input/Input"
import { useGameProviderList } from "../../game/providers/client"
import {
    AddGameProviderButton,
    EditGameProviderButton,
    ViewGameProviderDisclosure
} from "./GameProvider"
import { GameProviderStatusMenu } from "./GameProviderStatus"
import textColumn from "../../table/column/textColumn"
import { GameProviderGameListDisclosure } from "./GameProviderGameList"
import { GameProviderConfigListDisclosure } from "./GameProviderConfigList"
import AlertDialog from "../../dialog/Alert"

export { Code }

export const Content = () => {
    let isDisplay = useIsDisplayMenuByRoute()
    if (!isDisplay) {
        return null
    }

    return <Body />
}

export const CodeContentPair = [Code, Content]

const Body = () => {
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        code: "",
        name: "",
        sortby: "code"
    })
    let { pageIndex, pageSize, name } = state
    let { isLoading, data, error, mutate, deleteCache } = useGameProviderList({
        ...toApiParams(state),
        sortby: "code"
    })

    let columns = useColumns({ items: data?.items, mutate })

    return (
        <PageContent
            title={<Trans zh="游戏商管理" en="Game Provider Management" />}
            actionBar={
                <SearchBar
                    name={name}
                    search={(data) => {
                        setState({
                            pageIndex: 0,
                            ...data
                        })
                        mutate()
                    }}
                    mutate={mutate}
                />
            }
        >
            <Table
                columns={columns}
                data={data?.items}
                initialState={state}
                controlledState={state}
                pageCount={pageCount(pageSize, data?.totalCount)}
                setPageIndex={setPageIndex}
                setSortOrder={setSortOrder}
                isLoading={isLoading}
            />
            {error && <AlertDialog {...error} onClickClose={deleteCache} />}
        </PageContent>
    )
}

const SearchBar = ({
    code = "",
    name = "",
    search,
    isLoading,
    mutate,
    ...props
}) => {
    let formMethods = useForm({
        defaultValues: {
            code,
            name
        }
    })

    let codePlaceHolder = useTrans({
        zh: "游戏商代码",
        en: "game provider code"
    })

    let namePlaceHolder = useTrans({
        zh: "游戏商名称",
        en: "game provider name"
    })
    return (
        <Level>
            <FormProvider {...formMethods}>
                <form onSubmit={formMethods.handleSubmit(search)}>
                    <Set>
                        <InputField
                            name="code"
                            showEmptyFieldValueAddonAfter
                            placeholder={codePlaceHolder}
                        />
                        <InputField
                            name="name"
                            showEmptyFieldValueAddonAfter
                            placeholder={namePlaceHolder}
                        />
                        <SearchButton
                            isLoading={isLoading}
                            disabled={isLoading}
                        />
                    </Set>
                </form>
            </FormProvider>
            <AddGameProviderButton onSubmitted={mutate} />
        </Level>
    )
}

const useColumns = ({ items = [], mutate }) => {
    let hasUpdateAuth = useIsDisplayMenuByRoute([Update])
    let hasAnyMenu = hasUpdateAuth

    return useMemo(() => {
        let columns = [
            textColumn({
                accessor: "code",
                Header: <Trans zh="游戏商代码" en="Code" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "name",
                Header: <Trans zh="游戏商名称" en="Name" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "description",
                Header: <Trans zh="简介" en="Description" />,
                disableSortBy: true
            }),
            textColumn({
                id: "status",
                accessor: (row) => row,
                Header: <Trans zh="状态" en="Status" />,
                disableSortBy: true,
                Cell: ({ value }) => (
                    <GameProviderStatusMenu
                        record={value}
                        onSubmitted={mutate}
                    />
                )
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

        if (hasAnyMenu) {
            columns.push(
                textColumn({
                    id: "actions",
                    accessor: (row) => row,
                    disableSortBy: true,
                    Header: <Trans zh="操作" en="Actions" />,
                    Cell: ({ value }) => {
                        return (
                            <Set orientation="vertical" alignX="center">
                                <ViewGameProviderDisclosure
                                    gameProviderId={value?.id}
                                />
                                <EditGameProviderButton
                                    gameProviderId={value?.id}
                                    onSubmitted={mutate}
                                />
                                <GameProviderConfigListDisclosure
                                    gameProviderId={value?.id}
                                />
                                <GameProviderGameListDisclosure
                                    gameProviderId={value?.id}
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
