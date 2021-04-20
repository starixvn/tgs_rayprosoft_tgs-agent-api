import React, { useState, useEffect, useMemo } from "react"
import { css, Global } from "@emotion/react"
import {
    Heading,
    Stack,
    Button,
    Set,
    Text,
    Box,
    Input,
    Switch,
    Dialog
} from "bumbag"
import PageContent from "../../page/Content"
import sequenceColumn from "../../table/column/sequence"
import dateTimeColumn from "../../table/column/dateTime"
import {
    useIsDisplayMenuByRoute,
    useHasSubMenuByRoute,
    useUserBrandId
} from "../../user/context"
import { Code } from "./menu"
import { useTable, Table, pageCount, toApiParams } from "../../table"
import { Trans, useTrans } from "../../i18n"
import { SearchButton } from "../../button/SearchButton"
import equals from "fast-deep-equal"
import { useBrandGameList, useChangeBrandGameOrders } from "./client"
import { EditButton } from "./edit"
import { GameTypeLabel, GameTypeSelect } from "../games/GameType"
import { GamePlatformLabel } from "../games/GamePlatform"
import { GameTag } from "../games/GameTag"
import { GameTagSwitch } from "./GameTagSwitch"
import { GameStatusToggleSwitch } from "./GameStatusToggleSwitch"
import { useForm, FormProvider, useFormContext } from "react-hook-form"
import { InputField } from "../../input/Input"
import { BrandGameProviderSelect } from "../brandproviders/BrandGameProviderSelect"
import AlertDialog from "../../dialog/Alert"
import BrandBar from "../../report/BrandBar"
import { AllGameTypeSelect } from "../GameType"

export { Code } from "./menu"

export const Content = () => {
    let isDisplay = useIsDisplayMenuByRoute()
    if (!isDisplay) {
        return null
    }

    return <Body />
}

export const CodeContentPair = [Code, Content]

const Body = () => {
    let userBrandId = useUserBrandId()
    let [brandId, setBrandId] = useState(userBrandId)

    return (
        <Set orientation="vertical" isFilled>
            <BrandBar
                onBrandChange={(value) => {
                    setBrandId(value)
                }}
            />
            {brandId && <BodyContent brandId={brandId} />}
        </Set>
    )
}

const BodyContent = ({ brandId }) => {
    let { state, setState, setPageIndex, setSortOrder } = useTable({
        gameProviderId: "",
        gameType: "",
        keyword: ""
    })
    let { pageIndex, pageSize, gameProviderId, gameType, keyword } = state
    let { isLoading, data, error, mutate, deleteCache } = useBrandGameList({
        ...toApiParams(state),
        sortby: "displayOrder",
        brandId
    })

    return (
        <PageContent
            title={<Trans zh="游戏列表" en="Game List" />}
            actionBar={
                <SearchBar
                    brandId={brandId}
                    gameProviderId={gameProviderId}
                    gameType={gameType}
                    keyword={keyword}
                    search={(data) => {
                        let newState = {
                            pageIndex: 0,
                            ...data
                        }
                        if (
                            equals(newState, {
                                pageIndex,
                                keyword,
                                gameProviderId,
                                gameType
                            })
                        ) {
                            mutate()
                        }
                        setState(newState)
                    }}
                />
            }
        >
            <DataTable
                state={state}
                data={data}
                pageSize={pageSize}
                setPageIndex={setPageIndex}
                setSortOrder={setSortOrder}
                mutate={mutate}
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

const DataTable = ({
    data,
    pageSize,
    setPageIndex,
    setSortOrder,
    isLoading,
    mutate
}) => {
    let formMethods = useForm()
    let [displayOrders, setDisplayOrders] = useState({})

    const setDisplayOrder = (id, displayOrder) => {
        setDisplayOrders((state) => {
            Reflect.set(state, id, displayOrder)
            return state
        })
    }

    let [submitData, setSubmit] = useState()

    let {
        result,
        isLoading: isChangingDisplayOrder,
        error
    } = useChangeBrandGameOrders(submitData)

    const onSubmit = (formData) => {
        let orders = []
        Object.entries(displayOrders).forEach(([id, value]) => {
            orders.push({
                id,
                displayOrder: value
            })
        })

        setSubmit({
            orders
        })
    }

    const reset = () => {
        setDisplayOrders({})
        setSubmit(undefined)
    }

    useEffect(() => {
        if (result && !error) {
            reset()
            mutate()
        }
    }, [result, error])

    const items = data?.items ?? []

    let columns = useColumns({
        items,
        mutate,
        formMethods,
        displayOrders,
        setDisplayOrder
    })

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(onSubmit)}>
                <Stack spacing="major-1">
                    <Table
                        columns={columns}
                        data={items}
                        pageCount={pageCount(pageSize, data?.totalCount)}
                        setPageIndex={setPageIndex}
                        setSortOrder={setSortOrder}
                        isLoading={isLoading}
                    />
                    <Dialog.Footer position="sticky" bottom="0">
                        <Button
                            isLoading={isChangingDisplayOrder}
                            type="submit"
                        >
                            <Trans zh="确认排序" en="Confirm Order" />
                        </Button>
                    </Dialog.Footer>
                </Stack>
            </form>
        </FormProvider>
    )
}

const SearchBar = ({
    brandId,
    gameProviderId = "",
    gameType = "",
    keyword = "",
    search,
    isLoading,
    ...props
}) => {
    let formMethods = useForm({
        defaultValues: {
            keyword: ""
        }
    })

    let searchPlaceHolder = useTrans({
        zh: "关键字输入框",
        en: "Input keyword"
    })

    const onSubmit = (data) => search(data)

    return (
        <FormProvider {...formMethods}>
            <form onSubmit={formMethods.handleSubmit(onSubmit)}>
                <Set>
                    <BrandGameProviderSelect
                        name="gameProviderId"
                        brandId={brandId}
                    />
                    <AllGameTypeSelect name="gameType" />
                    <InputField
                        name="keyword"
                        showEmptyFieldValueAddonAfter
                        placeholder={searchPlaceHolder}
                        autocomplete="off"
                    />
                    <SearchButton isLoading={isLoading} disabled={isLoading} />
                </Set>
            </form>
        </FormProvider>
    )
}

const useColumns = ({
    items = [],
    mutate,
    formMethods,
    displayOrders = {},
    setDisplayOrder
}) => {
    let { register, getValues } = formMethods
    let hasAnyMenu = useHasSubMenuByRoute()

    return useMemo(() => {
        let columns = [
            {
                id: "displayOrder",
                accessor: (record) => record,
                Header: <Trans zh="序" en="Order" />,
                disableSortBy: true,
                headCell: {
                    textAlign: "center",
                    whiteSpace: "nowrap"
                },
                cell: {
                    textAlign: "center"
                },
                Cell: ({ value }) => {
                    let { id } = value
                    let displayOrder = displayOrders[id] ?? value.displayOrder
                    let { ref: inputRef, ...formProps } = register(id, {
                        valueAsNumber: true
                    })
                    return (
                        <Input
                            name={id}
                            inputRef={inputRef}
                            type="number"
                            size="small"
                            min="0"
                            minLength="1"
                            width="5em"
                            defaultValue={displayOrder}
                            {...formProps}
                            onChange={(event) => {
                                let displayOrder = parseInt(event.target.value)
                                if (Number.isFinite(displayOrder)) {
                                    setDisplayOrder(id, displayOrder)
                                }
                            }}
                        />
                    )
                }
            },
            {
                accessor: "gameId",
                Header: <Trans zh="游戏ID" en="ID" />,
                disableSortBy: true,
                headCell: {
                    textAlign: "center",
                    whiteSpace: "nowrap"
                },
                cell: {
                    textAlign: "center"
                }
            },
            {
                accessor: "gameProviderName",
                Header: <Trans zh="厂商名称" en="Game Provider Name" />,
                disableSortBy: true,
                headCell: {
                    textAlign: "center",
                    whiteSpace: "nowrap"
                },
                cell: {
                    textAlign: "center"
                }
            },
            {
                accessor: "gameName",
                Header: <Trans zh="游戏名称" en="Game Name" />,
                disableSortBy: true,
                headCell: {
                    textAlign: "center"
                },
                cell: {
                    textAlign: "center"
                }
            },
            {
                accessor: "gameType",
                Header: <Trans zh="游戏类型" en="Game Type" />,
                disableSortBy: true,
                headCell: {
                    textAlign: "center",
                    whiteSpace: "nowrap"
                },
                cell: {
                    textAlign: "center"
                },
                Cell: ({ value }) => {
                    return (
                        <Text.Block whiteSpace="nowrap">
                            <GameTypeLabel value={value} />
                        </Text.Block>
                    )
                }
            },
            {
                accessor: "platform",
                Header: <Trans zh="支持设备" en="Platform" />,
                disableSortBy: true,
                headCell: {
                    textAlign: "center",
                    whiteSpace: "nowrap"
                },
                cell: {
                    textAlign: "center"
                },
                Cell: ({ value }) => {
                    return (
                        <Text.Block whiteSpace="nowrap">
                            <GamePlatformLabel value={value} />
                        </Text.Block>
                    )
                }
            },
            {
                id: "isHot",
                accessor: (record) => record,
                Header: <Trans zh="热门游戏" en="Hot" />,
                disableSortBy: true,
                headCell: {
                    textAlign: "center",
                    whiteSpace: "nowrap"
                },
                cell: {
                    textAlign: "center"
                },
                Cell: ({ value }) => {
                    let { id, isHot } = value
                    return (
                        <Text.Block whiteSpace="nowrap">
                            <GameTagSwitch
                                id={id}
                                tag={GameTag.Hot}
                                checked={isHot}
                                onSubmitted={mutate}
                            />
                        </Text.Block>
                    )
                }
            },
            {
                id: "isNew",
                accessor: (record) => record,
                Header: <Trans zh="最新游戏" en="New" />,
                disableSortBy: true,
                headCell: {
                    textAlign: "center",
                    whiteSpace: "nowrap"
                },
                cell: {
                    textAlign: "center"
                },
                Cell: ({ value }) => {
                    let { id, isNew } = value
                    return (
                        <Text.Block whiteSpace="nowrap">
                            <GameTagSwitch
                                id={id}
                                tag={GameTag.New}
                                checked={isNew}
                                onSubmitted={mutate}
                            />
                        </Text.Block>
                    )
                }
            },
            {
                id: "isRecommend",
                accessor: (record) => record,
                Header: <Trans zh="推荐游戏" en="Recommend" />,
                disableSortBy: true,
                headCell: {
                    textAlign: "center",
                    whiteSpace: "nowrap"
                },
                cell: {
                    textAlign: "center"
                },
                Cell: ({ value }) => {
                    let { id, isRecommend } = value
                    return (
                        <Text.Block whiteSpace="nowrap">
                            <GameTagSwitch
                                id={id}
                                tag={GameTag.Recommend}
                                checked={isRecommend}
                                onSubmitted={mutate}
                            />
                        </Text.Block>
                    )
                }
            },
            {
                id: "status",
                accessor: (record) => record,
                Header: <Trans zh="状态" en="Status" />,
                disableSortBy: true,
                headCell: {
                    textAlign: "center",
                    whiteSpace: "nowrap"
                },
                cell: {
                    textAlign: "center"
                },
                Cell: ({ value }) => {
                    let { id, status } = value
                    return (
                        <Box alignX="center">
                            <GameStatusToggleSwitch
                                id={id}
                                value={status}
                                onSubmitted={mutate}
                            />
                        </Box>
                    )
                }
            }
        ]

        //TODO: auth
        if (hasAnyMenu) {
            columns.push({
                id: "actions",
                disableSortBy: true,
                Header: <Trans zh="操作" en="Actions" />,
                headCell: {
                    textAlign: "center"
                },
                cell: {
                    textAlign: "center"
                },
                accessor: (row) => {
                    return row
                },
                Cell: ({ value }) => {
                    return (
                        <Set orientation="vertical" alignX="center">
                            <EditButton record={value} onSubmitted={mutate} />
                        </Set>
                    )
                }
            })
        }

        return columns
    }, [JSON.stringify(items)])
}
