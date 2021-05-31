import React, { useState, useEffect, useMemo } from "react"
import { css } from "@emotion/react"
import { Box, Button, Set, Icon, Flex } from "bumbag"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute,
    useUserRoleType,
    useUserRoleId
} from "../../user/context"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { Code } from "./menu"
import textColumn from "../../table/column/textColumn"
import dateTimeColumn from "../../table/column/dateTime"
import { useForm, FormProvider } from "react-hook-form"
import { InputField } from "../../input/Input"
import SearchButton from "../../button/SearchButton"
import { PageContent } from "../../page/Content"
import AlertDialog from "../../dialog/Alert"
import { Trans } from "../../i18n"
import { AddLangDisclosure, EditLangDisclosure } from "./edit"
import { useLanguageList } from "../../language/client"
import equals from "fast-deep-equal"

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
        cultureCode: "",
        name: ""
    })

    let { isLoading, data, error, mutate, deleteCache } = useLanguageList({
        ...toApiParams(state),
        sortby: "cultureCode"
    })

    let columns = useColumns({ mutate, items: data?.items })
    return (
        <PageContent
            title={<Trans zh="语系" en="Language" />}
            actionBar={
                <Flex justifyContent="space-between">
                    <SearchBar
                        cultureCode={state?.cultureCode}
                        languageCode={state?.languageCode}
                        search={(data = {}) => {
                            setState({
                                pageIndex: 0,
                                ...data
                            })
                            mutate()
                        }}
                        isLoading={isLoading}
                    />
                    <AddLangDisclosure onSubmitted={mutate} />
                </Flex>
            }
        >
            <Table
                controlledState={state}
                columns={columns}
                data={data?.items}
                pageCount={pageCount(state?.pageSize, data?.totalCount)}
                setPageIndex={setPageIndex}
                setSortOrder={setSortOrder}
                isLoading={isLoading}
            />
            {error && (
                <AlertDialog
                    {...error}
                    onClickClose={() => {
                        deleteCache()
                        mutate({ data: undefined }, false)
                    }}
                />
            )}
        </PageContent>
    )
}

const SearchBar = ({
    cultureCode = "",
    languageCode = "",
    search,
    isLoading
}) => {
    let formMethods = useForm({
        defaultValues: { cultureCode, languageCode }
    })

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(search)}>
                <Set>
                    <InputField
                        name="cultureCode"
                        showEmptyFieldValueAddonAfter
                        placeholder="Culture Code"
                    />
                    <InputField
                        name="languageCode"
                        showEmptyFieldValueAddonAfter
                        placeholder="Language Code"
                    />
                    <SearchButton isLoading={isLoading} disabled={isLoading} />
                </Set>
            </form>
        </FormProvider>
    )
}

const useColumns = ({ items = [], mutate }) => {
    return useMemo(
        () => [
            textColumn({
                accessor: "cultureCode",
                Header: <Trans zh="Culture Code" en="Culture Code" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "languageCode",
                Header: <Trans zh="Language Code" en="Language Code" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "name",
                Header: <Trans zh="Name" en="Name" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "localizedName",
                Header: <Trans zh="Localized Name" en="Localized Name" />,
                disableSortBy: true
            }),
            dateTimeColumn({
                accessor: "createdOn",
                Header: <Trans zh="创建时间" en="Updated Time" />,
                disableSortBy: true
            }),
            dateTimeColumn({
                accessor: "updatedOn",
                Header: <Trans zh="更改时间" en="Updated Time" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "createdByUsername",
                Header: <Trans zh="创建人" en="Created By" />,
                disableSortBy: true
            }),
            textColumn({
                accessor: "updatedByUsername",
                Header: <Trans zh="更新人" en="Updated By" />,
                disableSortBy: true
            }),
            textColumn({
                id: "actions",
                Header: <Trans zh="操作" en="Actions" />,
                accessor: (row) => row,
                disableSortBy: true,
                Cell: ({ value: record }) => {
                    return (
                        <Set orientation="horizontal">
                            <EditLangDisclosure
                                record={record}
                                onSubmitted={mutate}
                            />
                        </Set>
                    )
                }
            })
        ],
        [JSON.stringify(items)]
    )
}
